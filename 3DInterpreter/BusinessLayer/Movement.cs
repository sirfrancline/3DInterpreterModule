using BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Movement : IMovement
    {
        public  int CalculateMinStepPerMs(double mmps, double timeInMs, double minMpS)

        {
            double distance = timeInMs * mmps;

            int results = Convert.ToInt32(distance * minMpS);

            Console.WriteLine("the result is :  " + results);
            return results;
        }

        public void DisplayNumberOfSteps(double min, double timeInMs, double NumStepPerMms)
        {
            int total = 0;

            for (double i = 0; i < min; i += 0.1)
            {
                int step = Convert.ToInt32(NumStepPerMms * (i * 10));

                total += step;

                Console.WriteLine("time  is :  " + i + "  Step per mms :   " + step + "    Total steps per second " + total);
            }
        }

    }
}
