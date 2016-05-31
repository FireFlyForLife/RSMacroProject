using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowPainterLib;
using WindowPainterLib.Extra;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using Plasmoid.Extensions;
using RSMacroProgram.Api;
using RSMacroProgramApi.MacroApi.RS;

namespace RSMacroProgram
{
    public class MacroMain
    {
        #region propperties
        public RSGame game { set { Configuration.game = value; } get { return Configuration.game; } }
        public Process gameProcess { private set; get; }
        public WindowOverlay overlay { private set; get; }
        public Mouse mouse { private set; get; }
        public ScriptController ScriptExecuter { private set { scriptController = value; } get { return scriptController; } }
        #endregion
        public MainWindow window;
        private ScriptController scriptController;
        internal int adheight = 0;

        public MacroMain(RSGame game) {
            window = new MainWindow(game, this);
            window.Show();

            scriptController = new ScriptController(this);

            RSMacroProgram.Api.Configuration.game = game;
            /*var engine = new ScriptEngine();
            var session = engine.CreateSession();

            session.Execute(@"System.Console.WriteLine(""Hello Roslyn"");");*/
            
            if (gameProcess != null) setOverlay(gameProcess);
        }

        public void setOverlay(Process process) {
            if(overlay != null)
                overlay.Dispose();

            gameProcess = process;
            overlay = new WindowOverlay(process);
            overlay.OverlayPaint += paint;
            overlay.OverlayPaint += new Api.OSRS.Inventory(null).paint;
            overlay.Show();
            mouse = overlay.MouseInstance;
            mouse.Enable();
        }

        public void playScript(String path) {
            scriptController.setScript(path);
        }

        public void stopScript() {
            scriptController.setScript(null);
        }

        public ScriptInformation getScript() {
            return scriptController.getScript();
        }

        private void paint(Graphics g) {
            Pen pen = new Pen(Color.Green, 3);
            g.DrawRectangle(pen, new Rectangle(0, 0, overlay.Width - 1, overlay.Height - 1));
            //g.DrawRoundedRectangle(pen, new Rectangle(0, 0, overlay.Width - 1, overlay.Height - 1), 35);

            pen = new Pen(Color.LightGreen, 1);

            int x = (Configuration.screen.Width - Configuration.OSRS.NonResizableViewport.Width) / 2;
            
            //draw over the Ad Banner
            g.FillRectangle(Brushes.Black, new Rectangle(new Point(x + 2, 0), Configuration.AdBanner));
            g.DrawRectangle(new Pen(Color.White), new Rectangle(new Point(x + 2, 0), Configuration.AdBanner));

            if (game != null && game.Equals(RSGame.OSRS)) {
                if (Configuration.OSRS.resizable) {
                    throw new NotImplementedException();
                } else {
                    Point location = new Point(x, Configuration.AdBanner.Height);
                    g.DrawRectangle(pen, new Rectangle(location, Configuration.OSRS.NonResizableViewport));

                    Api.OSRS.Inventory inv = new Api.OSRS.Inventory(null);

                    //g.DrawRectangle(pen, Configuration.OSRS.viewport.Right - Api.OSRS.Inventory.FromBottomRightToFirstSlot.X,
                    //    Configuration.OSRS.viewport.Bottom - Api.OSRS.Inventory.FromBottomRightToFirstSlot.Y, 
                    //    Api.OSRS.Inventory.slotSize,
                    //    Api.OSRS.Inventory.slotSize);
                }
            }else if (game.Equals(RSGame.RS3) || game.Equals(RSGame.DS)) {
                
            }
        }

    }
}
