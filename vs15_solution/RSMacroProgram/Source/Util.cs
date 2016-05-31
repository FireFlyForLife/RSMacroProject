using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSMacroProgram
{
    //public class RSGame
    //{
    //    public static RSGame RS3 = new RSGame("Runescape 3", "jagex-jav://www.runescape.com/jav_config.ws");
    //    public static RSGame OSRS = new RSGame("Old School RuneScape", "jagex-jav://oldschool8.runescape.com/jav_config.ws");
    //    public static RSGame DS = new RSGame("DarkScape", "jagex-jav://www.runescape.com/jav_config_beta.ws");
    //    public static readonly RSGame[] games = new RSGame[] { RS3, OSRS, DS };

    //    private readonly String name, url;

    //    public RSGame(String name, String url)
    //    {
    //        this.name = name;
    //        this.url = url;
    //    }

    //    public String getName() {  return name;  }

    //    public String getUrl() {  return url; }

    //    public override string ToString()
    //    {
    //        return getName();
    //    }

    //    public override bool Equals(object obj) {
    //        if (obj is RSGame) {
    //            RSGame target = (RSGame)obj;
    //            return target.name == name && target.url == url;
    //        }
    //        return false;
    //    }

    //    public override int GetHashCode() {
    //        return base.GetHashCode();
    //    }
    //}  

    public class ProcessItem
    {
        public readonly Process process;

        public ProcessItem(Process proc)
        {
            process = proc;
        }

        public override string ToString()
        {
            return process.Id.ToString() + " - " + process.ProcessName;
        }
    }

    public static class Util
    {
        public static readonly String launcherName = "JagexLauncher";

        public static void sendHiddenCommand(String argument)
        {
            Console.WriteLine(argument);

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + argument;
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void startProcess(String path)
        {
            if (!File.Exists(path)) path += ".url";
            Console.WriteLine("Opening File: " + path);

            if (File.Exists(path))
            {
                Process.Start(path);
            }
        }

        public static String createTempUrl(string linkName, string linkUrl)
        {
            String location = Path.GetTempPath();

            if (!File.Exists(location + linkName))
            {
                using (StreamWriter writer = new StreamWriter(location + "\\" + linkName + ".url"))
                {
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=" + linkUrl);
                    writer.Flush();
                }
            }
            return location + linkName;
        }
    }
}
