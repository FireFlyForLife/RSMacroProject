using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSMacroProgram.Api.Exceptions
{
    public class HookingException : System.Exception
    {
        public HookingException() { }
        public HookingException(String message) : base(message) { }
        public HookingException(String message, HookingException innerException) : base(message, innerException) { }
    }
}
