using RSMacroProgramApi.MacroApi.Generic;
using System;
using System.Collections.Generic;
using System.Text;


namespace RSMacroProgramApi.MacroApi.RS.OSRS
{
    public abstract class OSRSScript : AutoScript
    {
        //TODO: what instances do I need here?
        public Inventory inventory;

        public override IInteractionObject api
        {
            get
            {
                return base.api;
            }

            set
            {
                if (base.api == null) {
                    base.api = value;
                    inventory = new Inventory(value);
                }
            }
        }
    }
}
