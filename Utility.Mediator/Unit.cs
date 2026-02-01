using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mediator
{
    /// <summary>
    /// Represents a void response for commands that don't return a value.
    /// </summary>
    public readonly struct Unit
    {
        public static readonly Unit Value = new();
    }
}
