using ConsoleApplication1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Untrusted
{
    public class Class1
    {
        public void test() {
            Console.WriteLine("Test");
        }

        
        public void move() {
            WinAPI.SetCursorPos(10, 10);
        }
    }
}
