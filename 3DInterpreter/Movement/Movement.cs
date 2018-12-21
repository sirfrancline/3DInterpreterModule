using PrinterConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement
{
   public class Movement
    {
        public List<StepData> HeadSteps { get; set; } = new List<StepData>();
        public List<StepData> BodySteps { get; set; } = new List<StepData>();
        public List<StepData> TailSteps { get; set; } = new List<StepData>();
        public float StartPoint { get; set; }
        public float EndPoint { get; set; }
        public int StepNumber { get; set; }
    }
}
