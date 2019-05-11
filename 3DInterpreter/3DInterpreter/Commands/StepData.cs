using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DInterpreter.Commands
{
    public class StepData
    {
        public decimal TimeStamp { get; set; }
        public decimal PositionAfterStep { get; set; }
        public decimal StepTime { get; set; }
        public decimal SpeedAfterMove { get; set; }
        public decimal DistanceAfterStep { get; set; }
        public int StepNumber { get; set; }
        public string AxisName { get; set; }
    }
}
