using RSMacroProgram.Api;
using RSMacroProgram.Api.Type;
using RSMacroProgramApi.MacroApi.Generic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = RSMacroProgramApi.MacroApi.Generic.Point;

namespace RSMacroProgram.Api.RS3
{
    //[Serializable]
    //public class RS3Api : BaseApi
    //{
    //    public readonly Inventory inventory;

    //    public RS3Api(InterractionObject api) : base(api) {
    //        inventory = Inventory.get;
    //    }
    //}

    [Serializable]
    public class Inventory : Paintable
    {
        public static readonly int ROWS = 4;
        public static readonly int COLOMN = 7;
        public static readonly int MAX_SLOTS = 28;

        #region Singleton
        private static Inventory instane;

        public static Inventory get {
            private set { instane = value; }
            get {
                if (instane == null)
                    instane = new Inventory(new Point(20, 20), 100, 100);
                return instane; }
        }
        #endregion

        private Slot[] slots;

        Point offset;
        int width;
        int height;

        private Inventory(Point offset, int width, int height) {
            this.offset = offset;
            this.width = width;
            this.height = height;

            slots = new Slot[MAX_SLOTS - 1];
            for(int i = 0; i < MAX_SLOTS -1; i++) {
                slots[i] = new Slot(i+1);
            }


        }

        public void paint(Graphics g) {
            Pen pen = new Pen(Color.White, 3);
            g.DrawRectangle(pen, offset.X, offset.Y, width, height);
            pen = new Pen(Color.LightBlue, 1);
            foreach (Slot slot in slots) {
                g.DrawRectangle(pen, offset.X + slot.position.X * Slot.size, offset.Y + slot.position.Y * Slot.size, Slot.size, Slot.size);
            }
        }

        public class Slot
        {
            public static readonly int size = 25;

            int index;

            public Slot(int index) {
                if (index < 1 || index > MAX_SLOTS)
                    throw new ArgumentOutOfRangeException("Param 'index' should have a value between 1 and 28");
                
                this.index = index;
            }

            public Point position {
                private set {
                    index = value.Y * ROWS + value.X;
                }

                get {
                    float fIndex = index -1;
                    int x = ((index - 1) % ROWS) + 1;
                    int y = (int)(Math.Floor( fIndex / ROWS ) + 1);
                    return new Point(x, y);
                }
            }

        }
    }
}
