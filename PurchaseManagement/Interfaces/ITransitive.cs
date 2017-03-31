using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Interfaces
{
    public interface ITransitive
    {
        //This interface is for element that is navigate by frame
        //Implement this interface can pass data easy
        void Transmit(object obj);
        object Submit();
    }
}
