using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterConfiguration
{
    public class Configuration
    {

        public float XStepsPerMM { get; set; } = 200;
        public float XMaxSpeedPerMM { get; set; } = 30;

        public float YStepsPerMM { get; set; } = 200;
        public float ZStepsPerMM { get; set; } = 200;
        public int XMaxAcceleration { get; set; } = 40;
        public int XJerk { get; set; } = 20;
        public int YMaxAcceleration { get; set; } = 60;
    }
}
