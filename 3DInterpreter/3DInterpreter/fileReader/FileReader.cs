using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DInterpreter.fileReader
{
  
    public class FileReader
    {
        public List<string> Readfile(string path = "eat_sleep_code_repeat.gcode")
        {
            var lines = System.IO.File.ReadAllLines(path);
            var returnValue = new List<string>();
            foreach (var line in lines)
            {
                // trime only once to remove trailing and ending spaces
                var trimmed = line.Trim();

                // remove lines with comments
                if (trimmed.StartsWith(";"))
                    continue;

                // remove empty lines
                if (trimmed.Length == 0)
                    continue;

                returnValue.Add(trimmed);
            }

            return returnValue;

        }
    }
}
