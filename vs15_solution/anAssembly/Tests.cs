using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSMacroProgramApi.MacroApi.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anAssembly
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void FailTest() {
            Assert.IsInstanceOfType(new Class1(), typeof(AbstractScript));
            Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in types) {
                Console.WriteLine(t.FullName);
                Console.WriteLine(t.IsSubclassOf(typeof(AbstractScript)));
            }
            Assert.IsTrue(true);
        }
    }

}
