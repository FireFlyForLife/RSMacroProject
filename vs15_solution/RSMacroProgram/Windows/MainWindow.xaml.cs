using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WindowPainterLib;
using WindowPainterLib.Extra;

namespace RSMacroProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MacroMain main;
        private Random random;

        private String gameString;
        private String scriptString;
        private String authorString;
        private String descString;

        private ScriptInformation sInfo;

        private IntPtr mouseHook;
        private Hotkey_WPF stopHotkey;

        public MainWindow(RSGame game, MacroMain main) {
            /*this.game = game;
            this.gameProcess = process;

            InitializeComponent();

            consoleOutput.AppendText(Environment.NewLine);

            TextWriter t = Console.Out;
            Console.SetOut(new TextBoxWriter(consoleOutput));

            consoleOutput.IsReadOnly = true;

            selectProcess.Click += new RoutedEventHandler(printaline);*/
            init(game, main);
        }

        public MainWindow() {
            init(RSGame.RS3, new MacroMain(RSGame.RS3));
        }

        void init(RSGame game, MacroMain main) {
            this.main = main;

            this.random = new Random();

            InitializeComponent();

            consoleOutput.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            //consoleOutput.AppendText(Environment.NewLine);

            foreach (Process p in Process.GetProcesses()) {
                ProcessItem i = new ProcessItem(p);
                processSelectBox.Items.Add(i);
                if (p.ProcessName == Util.launcherName) {
                    processSelectBox.SelectedItem = i;
                    main.setOverlay(p);
                    break;
                }
            }
            /*if(main.gameProcess != null) {
                main.overlay = new WindowOverlay(main.gameProcess);
                main.overlay.Show();
                this.mouse = main.overlay.MouseInstance;
                this.mouse.Enable();
            }*/

            foreach (RSGame g in RSGame.games) {
                MenuItem item = new MenuItem();
                item.Header =  g.getName();
                item.IsCheckable = true;
                item.Click += new RoutedEventHandler(setGame);
                if (g.Equals(game))
                    item.IsChecked = true;
                gameMenu.Items.Add(item);
            }

            mouseHook = IntPtr.Zero;

            gameString = (String)lblGame.Content;
            scriptString = (String)lblScript.Content;
            authorString = (String)lblAuthor.Content;
            descString = (String)lblDesc.Content;

            lblGame.Content += game.getName();
            Closing += onClose;

            //TextWriter t = Console.Out;
            //Console.SetOut(new TextBoxWriter(consoleOutput));

            consoleOutput.IsReadOnly = true;

            Polygon pol = new Polygon();
            pol.Stroke = System.Windows.Media.Brushes.Black;
            pol.Fill = System.Windows.Media.Brushes.LightSeaGreen;
            pol.StrokeThickness = 2;
            pol.HorizontalAlignment = HorizontalAlignment.Left;
            pol.VerticalAlignment = VerticalAlignment.Center;
            Point p1 = new Point(10, 10);
            Point p2 = new Point(100, 15);
            Point p3 = new Point(86, 40);
            Point p4 = new Point(136, 76);
            PointCollection col = new PointCollection();
            col.Add(p1);
            col.Add(p2);
            col.Add(p3);
            col.Add(p4);
            pol.Points = col;

            //processCanvas.Children.Add(pol);
            
            //selectProcess.Click += new RoutedEventHandler(selectTheProcess);
            //refreshProcess.Click += new RoutedEventHandler(refreshProcesses);
            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {

        }

        private void onClose(object sender, CancelEventArgs e) {
            main.playScript(null);
        }

        public void addText(String text) {
            consoleOutput.Text += Environment.NewLine + text;
            consoleOutput.ScrollToEnd();
        }

        public void updateScriptInfo(ScriptInformation information) {
            sInfo = information;
            lblScript.Dispatcher.BeginInvoke((Action)(() => lblScript.Content = scriptString + sInfo.attribute.name));
            lblAuthor.Dispatcher.BeginInvoke((Action)(() => lblAuthor.Content = authorString + sInfo.attribute.author));
            lblDesc.Dispatcher.BeginInvoke((Action)(() => lblDesc.Content = descString + sInfo.attribute.description));

        }

        void TestHandler(Object sender, EventArgs e) {

        }

        internal static IntPtr LL_MouseCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            Debug.WriteLine("Mouse callback");
            return IntPtr.Zero;
        }

        void selectTheProcess(Object sender, EventArgs e) {
            if (processSelectBox.SelectedItem == null)
                addText("No process selected, no action will  be taken.");
            else {
                ProcessItem item = (ProcessItem)processSelectBox.SelectedItem;
                if (main.gameProcess != item.process || true) {
                    /*main.gameProcess = item.process;

                    main.overlay.Dispose();

                    main.overlay = new WindowOverlay(main.gameProcess);
                    main.overlay.Show();
                    this.mouse = main.overlay.MouseInstance;
                    this.mouse.Enable();*/
                    main.setOverlay(item.process);

                    addText( "Selected process: " + item.ToString() );
                } else addText("This process is already selected.");
            }
        }

        void refreshProcesses(Object sender, EventArgs e) {
            foreach (Process p in Process.GetProcesses()) {
                ProcessItem i = new ProcessItem(p);
                processSelectBox.Items.Add(i);
                if (main.gameProcess != null && p.Id == main.gameProcess.Id) {
                    processSelectBox.SelectedItem = i;
                }
            }

            addText("Refreshed the process list.");
        }

        void setGame(Object sender, EventArgs e) {
            MenuItem item = (MenuItem)sender;
            foreach(RSGame g in RSGame.games) {
                if (g.getName().Equals(item.Header)) {
                    main.game = g;
                    lblGame.Content = "Game: " + g.getName();
                }
            }

            foreach(MenuItem i in gameMenu.Items) {
                if (i == sender)
                    i.IsChecked = true;
                else
                    i.IsChecked = false;
            }
            addText( "Selected game: " + main.game.ToString() );
        }

        private void chooseScript_Click(object sender, RoutedEventArgs e) {
            
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                Console.WriteLine(dialog.FileName);
                if (Hooking.Manager.hookLLInput())
                    main.playScript(dialog.FileName);
                else
                    Console.WriteLine("Failed to hook input, Script will not be started.");
                //mouseHook = Hooking.Manager.reHook(Win32.HookingIDs.WH_Mouse_LL, ScriptController.HookingStuff.LL_MouseCallback);
            }
        }

        private void stopScript_Click(object sender, RoutedEventArgs e) {
            main.stopScript();
            Hooking.Manager.stopLLInput();
        }

        private void scriptMenu_Click(object sender, RoutedEventArgs e) {
            stopScript.IsEnabled = main.getScript().run;
        }

        private void rsLogin_Click(object sender, RoutedEventArgs e) {

        }
    }
}
