using System;
using System.Collections.Generic;
using System.Text;

namespace RSMacroProgramApi.MacroApi.Generic
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptAttribute : Attribute
    {
        public String name { private set; get; }
        public String description { private set; get; }
        public String version { private set; get; }
        public String author { private set; get; }
        public bool pausable { private set; get; }

        public ScriptAttribute(String Name, String Description, String Version = "0.1", String Author = "", bool Pausable = false) {
            this.name = Name;
            this.description = Description;
            this.version = Version;
            this.author = Author;
            this.pausable = pausable;
        }
    }
}
