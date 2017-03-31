using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Interfaces
{
    public interface ITablePrint
    {
        //This Interface is for any element to implement print
        //Implement this interface, need to return one element of
        //which inheritance UIElement, so that it can be printed
        System.Data.DataTable GetPrintTable();
    }
}
