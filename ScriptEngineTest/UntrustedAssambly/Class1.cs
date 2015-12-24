using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UntrustedAssambly
{
    public class Class1
    {
        String message;

        public Class1() {
            message = "Hello Random app!";
        }

        public void setMessage(String newMessage) {
            message = newMessage;
        }

        public String getMessage(int times) {
            String s = "";
            for (int i = 0; i < times; i++) s += message + Environment.NewLine;
            return s;
        }
    }
}
