using System;
using System.Collections.Generic;
using System.Text;

namespace RSMacroProgramApi.MacroApi.RS
{
    public class RSGame
    {
        public static RSGame NXT = new RSGame("Runescape 3(NXT)", "rs-launch://www.runescape.com/k=5/l=$(Language:0)/jav_config.ws");
        public static RSGame RS3 = new RSGame("Runescape 3", "jagex-jav://www.runescape.com/jav_config.ws");
        public static RSGame OSRS = new RSGame("Old School RuneScape", "jagex-jav://oldschool8.runescape.com/jav_config.ws");
        public static RSGame DS = new RSGame("DarkScape :'(", "jagex-jav://www.runescape.com/jav_config_beta.ws");
        public static readonly RSGame[] games = new RSGame[] { NXT, RS3, OSRS, DS };

        private readonly String name, url;

        public RSGame(String name, String url) {
            this.name = name;
            this.url = url;
        }

        public String getName() { return name; }

        public String getUrl() { return url; }

        public override string ToString() {
            return getName();
        }

        public override bool Equals(object obj) {
            if (obj is RSGame) {
                RSGame target = (RSGame)obj;
                return target.name == name && target.url == url;
            }
            return false;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
