using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.View
{
    public enum SubmitType
    {
        Login = 0,
        Logout = 1,
        Error,
        Query = 2
    }

    public class SubmitEventArgs : EventArgs
    {
        private object[] handValues;
        private SubmitType type;
        public SubmitEventArgs(SubmitType type, params object[] handValues)
        {
            this.handValues = handValues;
            this.type = type;
        }
        public object[] HandValues
        {
            get { return handValues; }
        }
        public SubmitType Type
        {
            get { return type; }
        }
    }
}
