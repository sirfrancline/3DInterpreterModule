
using System;
using System.Linq;
using _3DInterpreter.Commands;
using System.Collections.Generic;
using _3DInterpreter.fileReader;
using Newtonsoft.Json;

namespace _3DInterpreter
{
    class Program
    {
        private static MovementCalculator _yAxisCalculator; //instantiating yAxis
        private static MovementCalculator _eAxisCalculator;
        private static MovementCalculator _xAxisCalculator;
        private static List<IPrinterCommand> _commandsList = new List<IPrinterCommand>();
        private static decimal _stepsmmx = 200;
        private static decimal _stepsmmy = 200;      
        private static decimal _stepsmme = 400;

        static void Main(string[] args)
        {
            InitializeAxes(); //inputing initial configurations
            var reader = new FileReader();
            // var lines = reader.Readfile(); // Calling the Read file methold which returns all the gcode commands

            List<string> lines;
            if (args.Length > 0)
            {
                // a cheat here, as we have our test file then change stps values.
                if (args[0].ToLower() == "syncsample.gcode")
                {

                    ChangeSteps();
                }

                lines = reader.Readfile(args[0]);
            }
            else
            {
                lines = reader.Readfile();
            }


            var noHomeSkip = true;
            var gdata = new GCodeData();
            var lineCounter = 1;
            foreach (var line in lines)
            {
                Console.WriteLine($"line {lineCounter++} from {lines.Count}"); // keeping track of current line
                Console.WriteLine("-------------");
                /*     M109 S240 ; wait for temperature to be reached  
                 *     ---------- > yhis will be taken as first element    
                 *     after second split
                [0]M109                  [1]s240           [2]""           * */
                var commands = line.Split(';')[0].Split(' ');
                if (commands[0].StartsWith("M"))
                {
                    ProcesMachineRelatedCommand(commands); // Writing the Machine (M) commands on screenrinter commands starting with M
                }
                if (commands[0] == "G28") // Reference Point
                {
                    Console.WriteLine("executing homing....");
                    gdata.Coordinates[PrinterAxis.X] = 0;
                    gdata.Coordinates[PrinterAxis.Y] = 0;
                    gdata.Coordinates[PrinterAxis.Z] = 0;
                    gdata.Coordinates[PrinterAxis.E] = 0;
                    noHomeSkip = false;
                }
                if (noHomeSkip)
                {
                    // cannot move printer before home executed;
                    Console.WriteLine("no home executed");
                    continue;
                }

                var prev = gdata.Clone(); 

                if (commands[0] == "G01" || commands[0] == "G1")
                {
                    Console.WriteLine(line);
                    foreach (var c in commands)// looing through the commands
                    {
                        /*G1 X61.832 Y83.245  E22.14390          X=61.832       Y=83.245     E=22.14390      */
                        if (c.Trim().StartsWith("X"))
                        {
                            gdata.Coordinates[PrinterAxis.X] = decimal.Parse(c.Replace("X", string.Empty));
                        }
                        if (c.Trim().StartsWith("Y"))
                        {
                            gdata.Coordinates[PrinterAxis.Y] = decimal.Parse(c.Replace("Y", string.Empty));
                        }
                        if (c.Trim().StartsWith("Z"))
                        {
                            gdata.Coordinates[PrinterAxis.Z] = decimal.Parse(c.Replace("Z", string.Empty));
                        }

                        if (c.Trim().StartsWith("E"))
                        {
                            gdata.Coordinates[PrinterAxis.E] = decimal.Parse(c.Replace("E", string.Empty));
                        }
                    }

                    CalculateMovement(prev, gdata);
                }
            }

            Console.WriteLine($"move recorded: {_commandsList.Count}");

            System.IO.File.WriteAllLines("output.json", new List<string>());

            foreach (var printerCommand in _commandsList)
            {
                var data = JsonConvert.SerializeObject(printerCommand);
                System.IO.File.AppendAllText("output.json", data);
                System.IO.File.AppendAllText("output.json", "\n\r");
                Console.Write(".");
            }

        }

        private static void ChangeSteps()
        {
            _stepsmmx = 5;
            _stepsmmy = 5;
        }
        private static void CalculateMovement(GCodeData prev, GCodeData gdata)
        {
            InitializeAxes();
            var stpsDict = new Dictionary<PrinterAxis, Movement>();
            var stepsX =
                _xAxisCalculator.CalculateSteps(prev.Coordinates[PrinterAxis.X], gdata.Coordinates[PrinterAxis.X], 200);

            var stepsY =
                _yAxisCalculator.CalculateSteps(prev.Coordinates[PrinterAxis.Y], gdata.Coordinates[PrinterAxis.Y], 200);

            var stepsZ =
                _yAxisCalculator.CalculateSteps(prev.Coordinates[PrinterAxis.Z], gdata.Coordinates[PrinterAxis.Z], 200);

            var stepsE =
                _eAxisCalculator.CalculateSteps(prev.Coordinates[PrinterAxis.E], gdata.Coordinates[PrinterAxis.E], 200);


            var times = new[] { stepsE.TotalTime, stepsX.TotalTime, stepsY.TotalTime, stepsZ.TotalTime };
            var maxTime = times.Max();

            stepsE.SpeedFactor = (stepsE.TotalTime != 0) ? maxTime / stepsE.TotalTime : 1;
            stepsZ.SpeedFactor = (stepsZ.TotalTime != 0) ? maxTime / stepsZ.TotalTime : 1;
            stepsX.SpeedFactor = 1;// (stepsX.TotalTime != 0) ? maxTime / stepsX.TotalTime : 1;
            stepsY.SpeedFactor = 1;// (stepsY.TotalTime != 0) ? maxTime / stepsY.TotalTime : 1;

            if (stepsX.TotalTime != 0)
            {
                CalculateTimeStamps(stepsX);
                stpsDict.Add(PrinterAxis.X, stepsX);
            }

            if (stepsY.TotalTime != 0)
            {
                CalculateTimeStamps(stepsY);
                stpsDict.Add(PrinterAxis.Y, stepsY);
            }

            if (stepsZ.TotalTime != 0)
            {
                CalculateTimeStamps(stepsZ);
                stpsDict.Add(PrinterAxis.Z, stepsZ);
            }

            if (stepsE.TotalTime != 0)
            {
                CalculateTimeStamps(stepsE);
                stpsDict.Add(PrinterAxis.E, stepsE);
            }

            _commandsList.Add(new MoveCommand(stpsDict));
        }
        private static void CalculateTimeStamps(Movement steps)
        {
            var timeFactor = 1000;
            decimal timestamp = 0;
            foreach (var step in steps.HeadSteps)
            {
                timestamp += (steps.SpeedFactor * step.StepTime * timeFactor);
                step.TimeStamp = timestamp;
            }

            foreach (var step in steps.BodySteps)
            {
                timestamp += (steps.SpeedFactor * step.StepTime * timeFactor);
                step.TimeStamp = timestamp;
            }

            foreach (var step in steps.TailSteps)
            {
                timestamp += (steps.SpeedFactor * step.StepTime * timeFactor);
                step.TimeStamp = timestamp;
            }
        }

        private static void ProcesMachineRelatedCommand(string[] commands)
        {
            Console.WriteLine("Machine command");
            foreach (var c in commands)
            {
                Console.WriteLine(c);
            }
        }

        private static void InitializeAxes()
        {
            _xAxisCalculator = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 500,
                MaxSpeedPerMM = 300,
                StepsPerMM = _stepsmmx
            },"X");


            _yAxisCalculator = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 500,
                MaxSpeedPerMM = 300,
                StepsPerMM = _stepsmmy
            },"Y");

            _eAxisCalculator = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 15,
                MaxSpeedPerMM = 20,
                StepsPerMM = _stepsmme
            },"E");
        }
    }
}