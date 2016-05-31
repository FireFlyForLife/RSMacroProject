using RSMacroProgramApi.MacroApi.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSMacroProgramApi.MacroApi.RS.RS3
{
    public abstract class RS3Script : AutoScript
    {
        //TODO: what instances do I need here?

        public override IInterractionObject api
        {
            get
            {
                return base.api;
            }

            set
            {
                base.api = value;
            }
        }
    }
}
