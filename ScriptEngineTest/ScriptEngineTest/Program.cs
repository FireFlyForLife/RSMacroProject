using Jails;
using Jails.Isolators.AppDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptEngineTest
{
    class Program
    {
        static void Main(string[] args) {
            Console.WriteLine("ScriptEngine Test...");

            var isolator = new AppDomainIsolator();
            var environment = new DefaultEnvironment();
            environment.Register(@"C:\Users\Maiko\Documents\GitHubVisualStudio\RSMacroProject\ScriptEngineTest\UntrustedAssambly\bin\Debug\UntrustedAssambly.dll");
            using (var jail = Jail.Create(isolator, environment)) {
                int times = 2;
                do {
                    dynamic proxy = jail.Resolve("UntrustedAssambly.Class1");

                    string result = proxy.getMessage(times);

                    Console.WriteLine(result);
                    String str = Console.ReadLine();
                    int.TryParse(str, out times);
                } while (Console.ReadLine() != ""); 
            }
            var input = Console.ReadLine();
        }
    }

    class ScriptJail : IJail
    {
        private readonly IHost _host;

        internal ScriptJail(IHost host) {
            if (host == null) throw new ArgumentNullException("host");
            _host = host;
        }

        public object Resolve(string typeName) {
            if (string.IsNullOrEmpty(typeName)) {
                throw new ArgumentNullException("typeName");
            }

            var target = _host.ResolveTarget(typeName);
            return new DynamicTargetProxy(target);
        }

        public T Resolve<T>(string typeName) where T : class {
            var target = _host.ResolveTarget(typeName);
            var proxy = new TypedTargetProxy(target, typeof(T));

            return proxy.GetTransparentProxy() as T;
        }

        public void Dispose() {
            
        }

        public static IJail Create(IIsolator isolator, IEnvironment environment = null) {
            if (isolator == null) throw new ArgumentNullException("isolator");

            var host = isolator.Build(environment ?? new DefaultEnvironment());

            // Register things from environment

            return new ScriptJail(host);
        }

    }
}
