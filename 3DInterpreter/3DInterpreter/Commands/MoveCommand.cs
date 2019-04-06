using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DInterpreter.Commands
{
    public class MoveCommand : IPrinterCommand
    {
        public MoveCommand(Dictionary<PrinterAxis, Movement> move)
        {
            CommandType = CommandType.Motion;
            CommandData = move;
        }

        public CommandType CommandType { get; }
        public object CommandData { get; set; }
    }
}
