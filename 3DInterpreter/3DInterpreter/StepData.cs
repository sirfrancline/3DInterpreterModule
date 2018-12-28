using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DInterpreter
{
   public class StepData
    {
        public int TimeStamp { get; set; }
        public double HeadPositionAfterStep { get; set; }
        public double StepTime { get; set; }
        public double SpeedAfterMove { get; set; }
        public double DistanceAfterStep { get; set; }
        public int StepNumber { get; set; }
    }
}
