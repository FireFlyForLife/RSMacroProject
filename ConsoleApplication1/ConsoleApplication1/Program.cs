using Jails;
using Jails.Isolators.AppDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{ 
    public static class WinAPI
    {
        [DllImport("User32.dll")]
        
        public static extern long SetCursorPos(int x, int y);
    }

    class Program
    {
        

        static void Main(string[] args) {
            var appOptions = new AppDomainOptions();
            appOptions.Permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));

            var isolator = new AppDomainIsolator(appOptions);
            var environment = new DefaultEnvironment();
            environment.Register(@"C:\Users\Maiko\Documents\GitHubVisualStudio\RSMacroProject\ConsoleApplication1\Untrusted\bin\Debug\Untrusted.dll");

            using (var jail = Jail.Create(isolator, environment)) {
                dynamic proxy = jail.Resolve("Untrusted.Class1");
                proxy.test();
                proxy.move();
            }
        }

        
    }
}
