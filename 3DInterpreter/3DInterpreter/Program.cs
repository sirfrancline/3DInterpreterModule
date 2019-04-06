
using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using _3DInterpreter.Commands;
using System.Collections.Generic;
using _3DInterpreter.fileReader;

namespace _3DInterpreter
{
    class Program
    {
        private static MovementCalculator _yAxisCalculator;
        private static MovementCalculator _eAxisCalculator;
        private static MovementCalculator _xAxisCalculator;
        private static List<IPrinterCommand> _commandsList = new List<IPrinterCommand>();

        static void Main(string[] args)
        {
            InitializeAxes();
            var reader = new FileReader();
            var lines = reader.Readfile();

            var posX = 0.0;
            var posY = 0.0;
            var posZ = 0.0;
            var posE = 0.0;

            var noHomeSkip = true;
            var gdata = new GCodeData();
            var lineCounter = 1;
            foreach (var line in lines)
            {
                Console.WriteLine($"line {lineCounter++} from {lines.Count}");
                Console.WriteLine("-------------");
                var commands = line.Split(';')[0].Split(' ');

                if (commands[0].StartsWith("M"))
                {
                    ProcesMachineRelatedCommand(commands);
                }

                if (commands[0] == "G28")
                {
                    Console.WriteLine("executing homing....");
                    gdata.Coordinates[PrinterAxis.X] = 0.0;
                    gdata.Coordinates[PrinterAxis.Y] = 0.0;
                    gdata.Coordinates[PrinterAxis.Z] = 0.0;
                    gdata.Coordinates[PrinterAxis.E] = 0.0;
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
                    foreach (var c in commands)
                    {
                        if (c.Trim().StartsWith("X"))
                        {
                            gdata.Coordinates[PrinterAxis.X] = double.Parse(c.Replace("X", string.Empty));
                        }

                        if (c.Trim().StartsWith("Y"))
                        {
                            gdata.Coordinates[PrinterAxis.Y] = double.Parse(c.Replace("Y", string.Empty));
                        }

                        if (c.Trim().StartsWith("Z"))
                        {
                            gdata.Coordinates[PrinterAxis.Z] = double.Parse(c.Replace("Z", string.Empty));
                        }

                        if (c.Trim().StartsWith("E"))
                        {
                            gdata.Coordinates[PrinterAxis.E] = double.Parse(c.Replace("E", string.Empty));
                        }
                    }

                    CalculateMovement(prev, gdata);
                }
            }

            Console.WriteLine($"move recorded: {_commandsList.Count}");
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

            stepsE.SpeedFactor = maxTime / stepsE.TotalTime;
            stepsZ.SpeedFactor = maxTime / stepsZ.TotalTime;
            stepsX.SpeedFactor = maxTime / stepsX.TotalTime;
            stepsY.SpeedFactor = maxTime / stepsY.TotalTime;

            if (stepsX.TotalTime != 0)
            {
                stpsDict.Add(PrinterAxis.X, stepsX);
            }

            if (stepsY.TotalTime != 0)
            {
                stpsDict.Add(PrinterAxis.Y, stepsY);
            }

            if (stepsZ.TotalTime != 0)
            {
                stpsDict.Add(PrinterAxis.Z, stepsZ);
            }

            if (stepsE.TotalTime != 0)
            {
                stpsDict.Add(PrinterAxis.E, stepsE);
            }

            _commandsList.Add(new MoveCommand(stpsDict));
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
                StepsPerMM = 1000
            });


            _yAxisCalculator = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 500,
                MaxSpeedPerMM = 300,
                StepsPerMM = 1000
            });

            _eAxisCalculator = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 15,
                MaxSpeedPerMM = 20,
                StepsPerMM = 1000
            });
        }
    }
}