using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DInterpreter.Commands
{
    public class StepData
    {
        public double TimeStamp { get; set; }
        public double PositionAfterStep { get; set; }
        public double StepTime { get; set; }
        public double SpeedAfterMove { get; set; }
        public double DistanceAfterStep { get; set; }
        public int StepNumber { get; set; }
    }
}
