using System;
using System.Collections.Generic;
using System.Text;

namespace RSMacroProgramApi.MacroApi.Generic
{
    public abstract class AbstractScript : IAbstractScript
    {
        protected IInteractionObject apiInstance;

        public virtual IInteractionObject api
        {
            set
            {
                if (apiInstance == null)
                    apiInstance = value;
            }

            get
            {
                return apiInstance;
            }
        }

        public IInteractionObject getApi() { return apiInstance; }

        public virtual void _setApi(IInteractionObject newApi) {
            Console.WriteLine("_setApi in AbstractScript");
            apiInstance = newApi;
        }

        /// <summary>
        /// This method is called at first and needs to returned for start() to be called.
        /// </summary>
        public abstract void init();

        /// <summary>
        /// This method is called after init() has returned and does not have to return for other methods to be called.
        /// </summary>
        public abstract void start();

        /// <summary>
        /// This method is continuesly called. After it is returned a 100 ms delay is added to prevent lagg. 
        /// </summary>
        public abstract void tick();

        /// <summary>
        /// This method is called when you script needs to stop. The program does not wait until this method has returned.
        /// </summary>
        public abstract void dispose();
    }
}
