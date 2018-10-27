using BusinessLayer;
using BusinessLayer.Interface;
using System;

namespace _3DInterpreter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            double timeInMs = 0.1;
            double min = 1;
            int total = 0;
            int step = 0;
            double minMm = 20;
            double mmps = timeInMs * minMm;
            

           

            IMovement movement = new Movement();

            int NumStepPerMs = movement.CalculateMinStepPerMs(mmps, timeInMs, minMm);
            movement.DisplayNumberOfSteps(min, timeInMs, NumStepPerMs);


        }

        //public static int CalculateMinStepPerMs(double mmps, double timeInMs, double minMpS)

        //{
        //    double distance = timeInMs * mmps;

        //    int results = Convert.ToInt32(distance * minMpS);

        //    Console.WriteLine("the result is :  " + results);
        //    return results;
        //}

        //public static void DisplayNumberOfSteps(double min, double timeInMs, double NumStepPerMms)
        //{
        //    int total =0;

        //    for (double i = 0; i < min; i += 0.1)
        //    {
        //        int step = Convert.ToInt32(NumStepPerMms * (i * 10));

        //        total += step;

        //        Console.WriteLine("time  is :  " + i  + "  Step per mms :   " + step + "    Total steps per second " + total);
        //    }
        //}

        // multiply time with speed will give u travel distance
    }
}