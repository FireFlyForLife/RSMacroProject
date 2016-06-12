using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RSMacroProgramApi.MacroApi.Generic
{
    public class GameScreen
    {
        IInteractionObject api;

        public GameScreen(IInteractionObject api) {
            this.api = api;
        }

        Bitmap Screenshot() {
            return Screenshot(0, 0, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
        }

        Bitmap GameScreenshot() {
            return Screenshot(api.TargetWindow.Location, api.TargetWindow.Size);
        }

        Bitmap Screenshot(int x, int y, int width, int height) {
            Bitmap bmp = new Bitmap(width, height);
            try {
                using (Graphics g = Graphics.FromImage(bmp)) {
                    g.CopyFromScreen(x, y, 0, 0, new Size(width, height));
                }
            } catch (System.ComponentModel.Win32Exception ex) {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
            }
            return bmp;
        }

        Bitmap Screenshot(System.Drawing.Point topLeft, System.Drawing.Size size) {
            return Screenshot(topLeft.X, topLeft.Y, size.Width, size.Height);
        }
    }
}
