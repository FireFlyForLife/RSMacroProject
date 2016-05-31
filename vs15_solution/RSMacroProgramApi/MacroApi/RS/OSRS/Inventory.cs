using System;
using System.Collections.Generic;
using System.Text;
using RSMacroProgramApi.MacroApi.Generic;
using System.Drawing;
using Point = RSMacroProgramApi.MacroApi.Generic.Point;

namespace RSMacroProgramApi.MacroApi.RS.OSRS
{
    public class Inventory : Paintable, Openable
    {
        #region Constants
        /// <summary>
        /// From the bottom right.
        /// </summary>
        public static readonly Point FromBottomRightToFirstSlot = new Point(202, 194);
        public static readonly Size margin = new Size(11, 5);
        public static readonly int slotSize = 31;
        public static readonly Rectangle Icon = new Rectangle(138, 238, 31, 33);
        #endregion
        IInterractionObject api;
        VirtualMouse mouse;
        private bool opened = true;
        private Random random;

        public Inventory(IInterractionObject api) {
            this.api = api;
            mouse = new VirtualMouse(api);
            random = new Random();
        }

        public Rectangle outline
        {
            get
            {
                //if (api.resizable)
                int x = api.TargetWindow.Right - FromBottomRightToFirstSlot.X;
                int y = api.TargetWindow.Bottom - FromBottomRightToFirstSlot.Y;
                int width = slotSize * 4;
                int height = slotSize * 7;
                return new Rectangle(x, y, width, height);
            }
        }

        public void paint(Graphics g) {
            g.DrawRectangle(Pens.AliceBlue, new Rectangle(api.TargetWindow.Right - Icon.X,
                        api.TargetWindow.Bottom - Icon.Y,
                        Icon.Width,
                        Icon.Height));

            if (isOpen()) {
                //if (Configuration.OSRS.resizable) {
                if (false) {
                    throw new NotImplementedException();
                } else {
                    Pen pen = new Pen(Color.LightCoral, 1);
                    for (int y = 0; y < 7; y++) {
                        for (int x = 0; x < 4; x++) {
                            int dX = api.TargetWindow.Right - FromBottomRightToFirstSlot.X + x * slotSize + x * margin.Width;
                            int dY = api.TargetWindow.Bottom - FromBottomRightToFirstSlot.Y + y * slotSize + y * margin.Height;
                            g.DrawRectangle(pen, dX, dY, slotSize, slotSize);
                        }
                    }

                }
            }
        }

        public Rectangle getBounds(int slotIndex) {
            if (slotIndex < 1 || slotIndex > 28)
                throw new ArgumentOutOfRangeException("slotIndex can only be between 1 and 28 included");

            float fIndex = slotIndex - 1;
            int x = ((slotIndex - 1) % 4) + 1;
            int y = (int)(Math.Floor(fIndex / 4) + 1);

            Console.WriteLine(slotIndex + " Has coords: " + x + ":" + y);

            return getBounds(new Point(x, y));
        }

        public Rectangle getBounds(Point slotCoordinate) {
            //Rectangle window = Configuration.screen;
            //Point lowerRight = new Point(window.Right, window.Bottom);
            Point topLeft = new Point(FromBottomRightToFirstSlot.X,
                FromBottomRightToFirstSlot.Y);

            return new Rectangle(topLeft.X + slotCoordinate.X * slotSize + slotCoordinate.X * margin.Width,
                topLeft.Y + slotCoordinate.Y * slotSize + slotCoordinate.Y * margin.Height,
                slotSize,
                slotSize);
        }

        public Point nextPoint(int slotIndex) {
            Rectangle bound = getBounds(slotIndex);

            return new Point(random.Next(bound.X, bound.Right), random.Next(bound.Y, bound.Bottom));
        }

        public Point nextPoint(Point slotCoord) {
            Rectangle bound = getBounds(slotCoord);

            return new Point(random.Next(bound.X, bound.Right), random.Next(bound.Y, bound.Bottom));
        }

        public void Open() {
            Rectangle view = api.TargetWindow;
            int x = view.Width - Icon.X;
            int y = view.Height - Icon.Y;

            bool result;

            result = mouse.Move(x + random.Next(0, Icon.Width), y + random.Next(0, Icon.Height));
            result = mouse.Click(0) && result;

            opened = result;
        }

        public bool isOpen() {
            return opened;
        }

        public void deselect() {
            opened = false;
        }
    }
}
