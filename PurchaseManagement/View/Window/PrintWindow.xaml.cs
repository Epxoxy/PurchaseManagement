using System.ComponentModel;
using System.Windows;
using Mairegger.Printing.Definition;
using PurchaseManagement.Extension;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        private System.Data.DataTable DataTable { get; set; }
        public PrintWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            printBtn.Click += printBtnClick;
            closeBtn.Click += closeBtnClick;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Loaded -= OnLoaded;
            printBtn.Click -= printBtnClick;
            closeBtn.Click -= closeBtnClick;
            base.OnClosing(e);
        }

        public PrintWindow(string title) :this()
        {
            this.titleBox.Text = title;
        }

        public PrintWindow(System.Data.DataTable datatable) : this()
        {
            this.DataTable = datatable;
        }

        public PrintWindow(System.Data.DataTable datatable, string title) : this()
        {
            this.titleBox.Text = title;
            this.DataTable = datatable;
        }
        
        private void printBtnClick(object sender, RoutedEventArgs e)
        {
            var pa = new PrintAppendixes();
            pa = pa | PrintAppendixes.Header;

            var printer = new Printer(pa, null);
            printer.Title = titleBox.Text;
            printer.Table = DataTable;
            printer.PreviewDocument();
            this.Close();
        }

        private void closeBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
