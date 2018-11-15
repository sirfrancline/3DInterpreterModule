using PrinterConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement
{
    public class MovementCalculator
    {

        private Configuration _printerConfiguration;
        public MovementCalculator(Configuration printerConfiguration)
        {
            _printerConfiguration = printerConfiguration;
        }

        public Movement CalculateX(int numbOfSteps)
        {
            // calculate distance
            
            double[] travelmmList = new double[ numbOfSteps];
            double[] time = new double[numbOfSteps];

            int minStepTime = 2 * 5 / 1000000;
            double speedPermm = 0;
            double speedmms = 0;

            // double XstepsPerMM = 200.0000;
            //  double XMaxAcceleration = 40.000;

            for (int i = 1; i <= numbOfSteps; i++)
            {
                numbOfSteps = i;

                travelmmList[i] = numbOfSteps * (1 / _printerConfiguration.XStepsPerMM);

                speedPermm = Math.Sqrt(2 * _printerConfiguration.XMaxAcceleration * travelmmList[i]);

                double pastValue = travelmmList[i - 1];

                //  double speedmm = Math.Sqrt(speedPermm * speedPermm + 2 * XMaxAcceleration * (travelmmList[i] - pastValue));

                time[i] = Math.Sqrt(2 * travelmmList[i] / _printerConfiguration.XMaxAcceleration);
                var stepTime = time[i] - time[i - 1];

                var canHandle = time[i] > minStepTime;
                Console.WriteLine($"number of steps: {numbOfSteps}" + $"  travelmm : {travelmmList[i]} " + $" speedPermm:  { speedPermm } " + $" time: {time[i]} " + $" step time : {stepTime} " + $" canHandle { canHandle}");
            }

            return null;
        }
    
}

    }

