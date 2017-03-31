using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Interfaces
{
    //Interface for MySQL create script
    public interface ISqlModel
    {
        bool ToScript(out Dictionary<string,string> columnValues);
    }
}
