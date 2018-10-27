using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
   public interface IMovement
    {
        int CalculateMinStepPerMs(double mmps, double timeInMs, double minMpS);
        void DisplayNumberOfSteps(double min, double timeInMs, double NumStepPerMms);
    }
}
