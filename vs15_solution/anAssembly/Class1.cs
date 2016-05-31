using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Security.Permissions;
using System.Security;
using System.Windows.Shapes;
using RSMacroProgramApi.MacroApi.Generic;
using Point = RSMacroProgramApi.MacroApi.Generic.Point;
using RSMacroProgramApi.MacroApi.RS.OSRS;

namespace anAssembly
{
    public class anotherclass
    {
        public class nesteldClass
        {

        }
    }

    public class something
    {
        private class privateNestedClass
        {

        }
    }

    public interface someInterface
    {

    }

    [Script("A Test Script", "Tests the external script things", "1.0", "Maiko Steeman")]
    public class Class1 : OSRSScript {
        private Random random;
        private bool stop;
        //private GameObject bank;
        //private OSRSApi os;

        public override void init() {
            Console.WriteLine("I AM NOW INITIALISING! :)");
            random = new Random();
            stop = false;
            //bank = new GameObject();
            //bank.gameX = 0;
            //bank.gameY = 0;
            //Polygon p0 = new Polygon();
            //p0.Points = new System.Windows.Media.PointCollection(new List<System.Windows.Point>() { new System.Windows.Point(), });
            //bank.bounds = new Polygon[0];
            //os = new OSRSApi(api);
            //inventory = new Inventory(api);
        }

        public override void start() {
            Console.WriteLine("Dont mind me just starting a script...");

            Point topleft = api.TargetWindow.Location;
            mouse.Move(0,0);

            Thread.Sleep(1000);

            Point bottomRight = new Point(api.TargetWindow.Width, api.TargetWindow.Height) ;
            mouse.Move(bottomRight);

            Thread.Sleep(1000);

            inventory.Open();

            Thread.Sleep(1000);

            Point slot1 = inventory.getBounds(1).Location;
            mouse.Move(slot1);
        }

        public override void tick() {
            if (stop)
                Console.WriteLine("Stopping in loop!");
        }

        public override void dispose() {
            Console.WriteLine("TESTSCRIPT is now exiting because AutoScript#dispose() is called");
            stop = true;
        }
    }

    public abstract class MyAutoScript : OSRSScript
    {

    }
}
