using RSMacroProgram.Api.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using Plasmoid.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = RSMacroProgram.Api.Type.Point;
using RSMacroProgram.Api;

namespace RSMacroProgram.Api.OSRS
{
    [Serializable]
    public class OSRSApi : RSMacroProgram.Api.BaseApi
    {
        public readonly Inventory inventory;

        public OSRSApi(InterractionObject api) : base(api) {
            inventory = new Inventory(api);
        }
    }

    //public static class Configuration
    //{
    //    public static bool resizable = false;
    //    public static Rectangle screen = Rectangle.Empty;

    //}
    [Serializable]
    public class Inventory : Paintable, Openable
    {
        //#region Singleton
        //private static Inventory instance;
        //public static Inventory get {
        //    private set { instance = value; }
        //    get {
        //        if (instance == null)
        //            instance = new Inventory();
        //        return instance;
        //    }
        //}
        //#endregion
        #region Constants
        /// <summary>
        /// From the bottom right.
        /// </summary>
        public static readonly Point FromBottomRightToFirstSlot = new Point(202, 194);
        public static readonly Size margin = new Size(11, 5);
        public static readonly int slotSize = 31;
        public static readonly Rectangle Icon = new Rectangle(138, 238, 31, 33);
        #endregion

        private int x, y, width, height;
        public Rectangle outline {
            set {
                x = value.Location.X;
                y = value.Location.Y;
                width = value.Size.Width;
                height = value.Size.Height;
            }
            get {
                if (Configuration.OSRS.resizable)
                    return new Rectangle(x - width, y - height, width, height);
                else
                    return new Rectangle(x - width, y - height, width, height);
            }
        }

        private bool opened = true;
        private Random random;

        private InterractionObject accessApi;

        public Inventory(InterractionObject api) {
            random = new Random();
            accessApi = api;
        }

        public void paint(Graphics g) {
            g.DrawRectangle(Pens.AliceBlue, new Rectangle(Configuration.OSRS.viewport.Right - Icon.X,
                        Configuration.OSRS.viewport.Bottom - Icon.Y,
                        Icon.Width,
                        Icon.Height));

            if (isOpen()) {
                if (Configuration.OSRS.resizable) {
                    throw new NotImplementedException();
                } else {
                    Pen pen = new Pen(Color.LightCoral, 1);
                    for(int y = 0; y < 7; y++) {
                        for(int x = 0; x < 4; x++) {
                            int dX = Configuration.OSRS.viewport.Right - FromBottomRightToFirstSlot.X + x * slotSize + x * margin.Width;
                            int dY = Configuration.OSRS.viewport.Bottom - FromBottomRightToFirstSlot.Y + y * slotSize + y * margin.Height;
                            g.DrawRectangle(pen, dX, dY, slotSize, slotSize);
                        }
                    }
                    
                }
            }
        }

        public Rectangle getBounds(int slotIndex) {
            if (slotIndex < 1 || slotIndex > 28)
                throw new ArgumentOutOfRangeException("slotIndex can only be between 1 and 28");

            float fIndex = slotIndex - 1;
            int x = ((slotIndex - 1) % 4) + 1;
            int y = (int)(Math.Floor(fIndex / 4) + 1);

            Console.WriteLine(slotIndex + " Has coords: " + x + ":" + y);

            return getBounds(new Point(x,y));
        }

        public Rectangle getBounds(Point slotCoordinate) {
            //Rectangle window = Configuration.screen;
            //Point lowerRight = new Point(window.Right, window.Bottom);
            Point topLeft = new Point(Configuration.OSRS.viewport.Right - FromBottomRightToFirstSlot.X, 
                Configuration.OSRS.viewport.Bottom - FromBottomRightToFirstSlot.Y);

            return new Rectangle(topLeft.X + slotCoordinate.X * slotSize + slotCoordinate.Y * margin.Width, 
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

            return new Point( random.Next(bound.X, bound.Right), random.Next(bound.Y, bound.Bottom) );
        }

        public void Open() {
            Rectangle view = Configuration.OSRS.viewport;
            int x = view.Right - Icon.X;
            int y = view.Bottom - Icon.Y;

            bool result;

            result = accessApi.Move(Configuration.screen.X + x + random.Next(0, Icon.Width), Configuration.screen.Y - y + random.Next(0, Icon.Height));
            result = accessApi.Click() && result;

            opened = result;
        }

        public bool isOpen() {
            return opened;
        }

        public void deselect() {
            opened = false;
        }

        public class Slot : SquareWidget
        {
            public static Point fromInventoryOffset = new Point(0,1);
            public static int slotMargin = 5;
            public static readonly int size = 25;

            private int index;
            private int row;
            private int column;

            public Slot(int index, int size = 25) : base( new Point( fromInventoryOffset.X * index * slotMargin, fromInventoryOffset.Y * index * slotMargin), size, size) {

            }

            public Slot(Point index, int size = 25) : base( new Point(), size, size) {

            }

            public void click() {

            }
        }
    }

    class Option
    {
        public static readonly int extraWidth = 10;
        public static readonly int minWidth = 20;
        private String content;

        public Option(String content = "") {
            this.content = content;
        }

        public int getWidth() {
            return content.Length * 5 + extraWidth;
        }
    }

    class OptionList : SquareWidget
    {
        public static readonly int baseHeight = 10;
        public static readonly int optionHeight = 8;

        private int optionWidth;
        private int optionCount;
        private Point origin;

        public OptionList(Point mousePos, int optionCount) : base(mousePos, Option.minWidth, getTotalHeight(optionCount)) {
            this.optionCount = optionCount;
            this.optionWidth = 25;
            this.origin = mousePos;
        }

        public OptionList(Point mousePos, Option[] options) : base(mousePos, Option.minWidth, getTotalHeight(options.Length)) {
            this.optionCount = options.Length;
            foreach (Option o in options) optionWidth = Math.Max(optionWidth, o.getWidth());
            this.origin = mousePos;
        }

        public int getOptionCount() { return optionCount; }

        public int getHeight(int options) { return baseHeight + optionHeight * options; }

        public int getHeight() { return baseHeight + optionHeight * optionCount; }

        public static int getTotalHeight(int options) { return baseHeight + optionHeight * options; }

        public Rectangle getDimension(int options) {
            return new Rectangle();
        }
    }
}
