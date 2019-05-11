using System.Collections.Generic;

namespace _3DInterpreter.Commands
{
    public class Movement
    {
        public List<StepData> HeadSteps { get; set; } = new List<StepData>();
        public List<StepData> BodySteps { get; set; } = new List<StepData>();
        public List<StepData> TailSteps { get; set; } = new List<StepData>();
    
        public List<StepData> AllSteps()
        {
            var steps = new List<StepData>();
            steps.AddRange(HeadSteps);
            steps.AddRange(BodySteps);
            steps.AddRange(TailSteps);

            return steps;
        }

        public decimal TotalTime { get; set; } = 0;
        public decimal SpeedFactor { get; set; }
        public bool Direction { get; set; } = true;
    }
}
