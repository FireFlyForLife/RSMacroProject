using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSMacroProgram.Api;
using System.Threading;
using RSMacroProgram.Api.OSRS;
using System.Drawing;
using System.Security.Permissions;
using System.Security;
using System.Windows.Shapes;
using RSMacroProgram.Api.Type;
using Point = RSMacroProgram.Api.Type.Point;

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

    [ScriptAttributes("A Test Script", "Tests the external script things", "1.0", "Maiko Steeman")]
    public class Class1 : MyAutoScript {
        private Random random;
        private bool stop;
        private GameObject bank;
        private OSRSApi os;
        private Inventory inventory;

        public override void init() {
            Console.WriteLine("I AM NOW INITIALISING! :)");
            random = new Random();
            stop = false;
            bank = new GameObject();
            bank.gameX = 0;
            bank.gameY = 0;
            //Polygon p0 = new Polygon();
            //p0.Points = new System.Windows.Media.PointCollection(new List<System.Windows.Point>() { new System.Windows.Point(), });
            //bank.bounds = new Polygon[0];
            os = new OSRSApi(api);
            inventory = new Inventory(api);
        }

        public override void start() {
            Console.WriteLine("Dont mind me just starting a script...");

            Point topleft = config.rsScreen.Location;
            os.mouse.Move(topleft);

            Thread.Sleep(1000);

            Point vpTopLeft = new Point(config.viewport.Location.X, config.viewport.Location.Y) ;
            os.mouse.Move(vpTopLeft);

            Thread.Sleep(1000);

            Point slot1 = os.inventory.getBounds(1).Location;
            os.mouse.Move(slot1);
        }

        public override void tick() {

        }

        public override void dispose() {
            Console.WriteLine("TESTSCRIPT is now exiting because AutoScript#dispose() is called");
            stop = true;
        }
    }

    public abstract class MyAutoScript : AutoScript
    {

    }
}
