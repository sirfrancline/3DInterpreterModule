using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DInterpreter.Commands
{
    public interface IPrinterCommand
    {
        // this interface helps us to have
        // list of commands that shall be executed
        CommandType CommandType { get; }
        object CommandData { get; set; }
    }
}
