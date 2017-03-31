using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PurchaseManagement.Extension
{
    public class Alert
    {
        private static Alert alert;
        private Alert()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
        }


        public static System.Windows.Forms.DialogResult show(string value, System.Windows.Forms.MessageBoxButtons buttons)
        {
            if (alert == null) alert = new Alert();
            return System.Windows.Forms.MessageBox.Show(value, Properties.Resources.Tips, buttons);
        }
        public static void show(string value)
        {
            if (alert == null) alert = new Alert();
            System.Windows.Forms.MessageBox.Show(value, Properties.Resources.Tips);
        }
    }
}
