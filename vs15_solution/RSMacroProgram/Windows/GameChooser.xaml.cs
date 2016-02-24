using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RSMacroProgram
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class GameChooser : Window
    {
        private bool clicked = false;

        public GameChooser()
        {
            //Tests.Test test = new Tests.Test();
            //Api.VirtualMouse.get.Move(100, 100);

            InitializeComponent();
            foreach (RSGame g in RSGame.games) gameChooser.Items.Add(g);

            //launchButton.Click += new RoutedEventHandler(launchGame);
        }

        void launchGame(Object sender, EventArgs e)
        {
            if (clicked)
                return;
            else
                clicked = true;

            RSGame game = (RSGame)gameChooser.SelectedItem;
            Console.WriteLine(game);

            String loc = Util.createTempUrl(game.ToString(), game.getUrl());
            Util.startProcess(loc);

            /*switch (game)
            {
                case "RuneScape 3":
                    String s = Util.createTempUrl("RS3", "jagex-jav://www.runescape.com/jav_config.ws");
                    Util.startProcess(s);
                    break;
                case "OldSchool RuneScape":
                    String t = Util.createTempUrl("OSRS", "jagex-jav://oldschool8.runescape.com/jav_config.ws");
                    Util.startProcess(t);
                    break;
                case "DarkScape":
                    String r = Util.createTempUrl("DarkScape", "jagex-jav://www.runescape.com/jav_config_beta.ws");
                    Util.startProcess(r);
                    break;
                default:
                    Console.WriteLine("Selected is not found.");
                    break;
            }*/
            MacroMain main = new MacroMain(game);
            this.Close();
        }

        void go(Object sender, EventArgs e) {
            RSGame game = (RSGame)gameChooser.SelectedItem;
            Console.WriteLine(game);

            MacroMain main = new MacroMain(game);
            this.Close();
        }

    }
}