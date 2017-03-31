using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Interfaces
{
    //Interface for UIElement so that dispose data, and unregister event handler
    interface IUIElement : IDisposable
    {
        /// <summary>
        /// Register event
        /// </summary>
        void EventsRegistion();

        /// <summary>
        /// Deregistrate event
        /// </summary>
        void EventsDeregistration();
    }
}
