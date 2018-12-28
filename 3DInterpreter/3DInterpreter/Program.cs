using Movement;
using System;
using _3DInterpreter.Configuration;

namespace _3DInterpreter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var xAxisCalculator = new MovementCalculator(new AxisConfiguration());
            var stepsX = xAxisCalculator.CalculateSteps(0, 100, 15);

            // calclateTotalTime
            var timeX = 0.00;
            foreach (var step in stepsX.HeadSteps)
            {
                timeX += step.StepTime;
            }

            foreach (var step in stepsX.BodySteps)
            {
                timeX += step.StepTime;
            }

            foreach (var step in stepsX.TailSteps)
            {
                timeX += step.StepTime;

            }

            var yAxisCalculator = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 500,
                MaxSpeedPerMM = 300
            });

            var stepsY = yAxisCalculator.CalculateSteps(34, 300, 300);
            var timeY = 0.00;

            foreach (var step in stepsY.HeadSteps)
            {
                timeY += step.StepTime;
            }

            foreach (var step in stepsY.BodySteps)
            {
                timeY += step.StepTime;
            }

            foreach (var step in stepsY.TailSteps)
            {
                timeY += step.StepTime;

            }

            Console.WriteLine($"TravelTimeX: {timeX}");
            Console.WriteLine($"TravelTimeY: {timeY}");
            Console.WriteLine("done");
        }
    }
}