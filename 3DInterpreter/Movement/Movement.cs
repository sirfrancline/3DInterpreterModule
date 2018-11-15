﻿using PrinterConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement
{
   public class Movement
    {
        public List<StepData> Steps { get; set; }
        public float StartPoint { get; set; }
        public float EndPoint { get; set; }
    }
}