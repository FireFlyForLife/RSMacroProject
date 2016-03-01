using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
using Jails.Isolators.AppDomain;
using Jails;
using RSMacroProgram.Api;
using System.Security;
using System.Security.Permissions;
using System.Windows.Interop;

namespace RSMacroProgram
{
    public class ScriptController
    {
        private MacroMain main;
        private ScriptInformation sInfo;

        private const bool debug = false;
        private Hotkey_WPF stopHotkey;

        /*private String assemblyName;
        private Type mainScript;
        private Type apiType;
        private ScriptAttributes attribute;
        private Assembly assembly;*/

        public ScriptController(MacroMain mainController) {
            main = mainController;

            sInfo = new ScriptInformation();
            sInfo.run = false;

            stopHotkey = new Hotkey_WPF(System.Windows.Forms.Keys.Q, false, true, false, false);
            stopHotkey.Pressed += (object sender, System.ComponentModel.HandledEventArgs e) => setScript(null);
            stopHotkey.Register(main.window);
        }

        private void Search() {
            sInfo.mainScript = getMainClass(sInfo.assembly);
            if (sInfo.mainScript == null) return;

            /*foreach(Type type in sInfo.mainScript.BaseType.GenericTypeArguments) {
                if( type.IsSubclassOf( typeof(Api) ) ) {
                    apiType = type;
                }
            }*/
            var scriptThread = new Thread(new ThreadStart(Run));
            scriptThread.Start();
        }

        private Type getMainClass(Assembly searchableAssembly) {
            try {
                foreach (Type t in searchableAssembly.GetTypes()) {
                    Console.WriteLine(t);
                    if (t.IsSubclassOf(typeof(AutoScript))) {
                        Console.WriteLine("Found a class extending AutoScript: " + t.FullName);
                        sInfo.attribute = (ScriptAttributes)Attribute.GetCustomAttribute(t, typeof(ScriptAttributes));
                        if (sInfo.attribute != null) {
                            Console.WriteLine("The class uses ScriptAttributes, name is: {0}", sInfo.attribute.name);
                            return t;
                        }
                    }
                }
            } catch (ReflectionTypeLoadException ex) {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }

        private void Run() {
            var appOptions = new AppDomainOptions();
            appOptions.Permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
            //appOptions.Permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.AllFlags));
            
            var isolator = new AppDomainIsolator(appOptions);
            var environment = new DefaultEnvironment();
            environment.Register(sInfo.assemblyPath);

            InterractionObject accessApi = new InterractionObject();
            DataAccessObject dataAccess = new DataAccessObject();

            using (var jail = Jail.Create(isolator, environment)) {
                var watch = new Stopwatch();
                var localSInfo = sInfo;
                main.window.updateScriptInfo(localSInfo);
                dynamic mainClass = jail.Resolve(sInfo.mainScript.FullName);
                mainClass.api = accessApi;
                mainClass.config = dataAccess;
                //localSInfo.mainClass = mainClass;

                new RunUntil(() => mainClass.init(), delegate () { return localSInfo.run; });
                //new Thread(() => mainClass.init());

                new Thread(() => mainClass.start()).Start();
                //localSInfo.mainClass.start();

                while (localSInfo.run) {
                    watch.Start();

                    /*Thread tick = new Thread(() => mainClass.tick());
                    tick.Start();
                    while (tick.IsAlive && localSInfo.run) Thread.Sleep(10);*/
                    new RunUntil(() => mainClass.tick(), delegate () { return localSInfo.run; });

                    var time = watch.ElapsedMilliseconds;
                    watch.Reset();

                    if(debug) Console.WriteLine(time);

                    Thread.Sleep(100);
                    // This will fail since the isolator has not been given
                    // FileIO permissions to that location.
                    //byte[] data = bar.ReadFile("C:\\Temp\\File.txt");
                }
                new Thread(() => mainClass.dispose()).Start();
            }
        }

        public void setScript(String pathToAssembly) {
            Console.WriteLine("Stopping current script...");
            sInfo.run = false;
            
            if (pathToAssembly != null) {
                Console.WriteLine("Setting up new script...");
                sInfo = new ScriptInformation();
                sInfo.assemblyPath = pathToAssembly;
                sInfo.assembly = Assembly.LoadFrom(sInfo.assemblyPath);

                Thread threadSearch = new Thread(new ThreadStart(Search));
                threadSearch.Start();
            } else {
                main.window.Dispatcher.BeginInvoke((Action)(() => Hooking.Manager.stopLLInput()));
            }
        }

        public ScriptInformation getScript() { return sInfo; }
    }

    public class RunUntil
    {
        public RunUntil(ThreadStart start, Func<bool> altReturn, int timeout = 10) {
            Thread thread = new Thread(start);
            thread.Start();

            while (thread.IsAlive && altReturn())
                Thread.Sleep(timeout);

            if (thread.IsAlive)
                thread.Abort();
        }
    }

    public class ScriptInformation
    {
        public String assemblyPath = "";
        public Type mainScript = null;
        public Type apiType = null;
        public ScriptAttributes attribute = null;
        public Assembly assembly = null;
        public bool run = true;

        public void reset() {
            assemblyPath = "";
            mainScript = null;
            apiType = null;
            attribute = null;
            assembly = null;
        }

        /*public void init() {
            
            mainClass.init();
        }

        public void start() {
            mainClass.start();
        }

        public void tick() {
            Console.WriteLine("Ticking the script");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            mainClass.tick();
            Console.WriteLine(watch.Elapsed);
            watch.Reset();
        }

        public void dispose() {
            mainClass.dispose();
        }*/
    }

    //public class ThreadInvoker
    //{
    //    ThreadStart threadStart;
        
    //    public ThreadInvoker() {

    //    }

    //    public ThreadInvoker(ThreadStart start) {
    //        threadStart = start;
    //    }

    //    public Thread Invoke() {
    //        if (threadStart == null) throw new ArgumentNullException("ThreadStart is null, either supply one through the constructor or in the Invoke() method.");

    //        Thread thread = new Thread(threadStart);
    //        thread.Start();

    //        return thread;
    //    }

    //    public Thread Invoke(ThreadStart start) {
    //        if (start == null) throw new ArgumentNullException("ThreadStart paramiter is null");

    //        Thread thread = new Thread(start);
    //        thread.Start();

    //        return thread;
    //    }
    //}
}
