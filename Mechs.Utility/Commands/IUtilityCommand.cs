using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechs.Utility.Commands
{
    internal interface IUtilityCommand
    {
        string CommandName { get; }

        void Execute(string[] args);
    }
}
