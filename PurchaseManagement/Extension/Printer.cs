using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManagement.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.PrintProcessor;
    using System.Data;

    public class Printer : PrintProcessor
    {
        private readonly IEnumerable<string> _collToPrint;
        private readonly PrintAppendixes _printingAppendix;
        private readonly UIElement printElement;
        public double TitleFontSize { get; set; } = 22;
        public string Title { get; set; } = "Title";
        public DataTable Table { get; set; }

        public Printer(UIElement uielement)
        {
            _printingAppendix = new PrintAppendixes();
            _collToPrint = new List<string>();
            printElement = uielement;
            FileName = "FileName";
        }
        
        public Printer(PrintAppendixes printingAppendix, UIElement uielement)
        {
            _printingAppendix = printingAppendix;
            _collToPrint = new List<string>();
            printElement = uielement;

            FileName = "FileName";
        }

        public override UIElement GetFooter()
        {
            return new Label { Content = "This is the footer" };
        }

        public override UIElement GetHeader()
        {
            return new TextBlock { Text = ""};
        }

        public override UIElement GetHeaderDescription()
        {
            return new Label { Content = "This the description of the header, left of the destinator" };
        }

        public override UIElement GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Summary" + Environment.NewLine);
            sb.Append("No elements: " + _collToPrint.Count() + Environment.NewLine);

            Label l = new Label
            {
                Content = sb.ToString(),
                Width = 600,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            return l;
        }

        public override UIElement GetTable(out double tableHeaderHeight, out Brush borderBrush)
        {
            borderBrush = null;
            //borderBrush = Brushes.Gray;
            tableHeaderHeight = 0;
            return new UIElement();
        }
        
        public override IEnumerable<IPrintContent> ItemCollection()
        {
            var printcontents = new List<IPrintContent>();
            printcontents.Add(new TextBlock { Text = Title, HorizontalAlignment = HorizontalAlignment.Center, FontSize = TitleFontSize }.ToPrintContent());
            if (Table != null)
            {
                var tables = Table.AsEnumerable().ToChunks(45).Select(rows => rows.CopyToDataTable());
                foreach (var table in tables)
                {
                    DataGrid datagrid = new DataGrid();
                    datagrid.IsReadOnly = true;
                    datagrid.HeadersVisibility = DataGridHeadersVisibility.Column;
                    datagrid.ItemsSource = table.DefaultView;
                    printcontents.Add(datagrid.ToPrintContent());
                    printcontents.Add(PrintContent.PageBreak());
                }
            }
            return printcontents;
        }

        protected override void PreparePrint()
        {
            base.PreparePrint();
            PrintDimension.Margin = new Thickness(50);

            if (_printingAppendix.HasFlag(PrintAppendixes.Summary))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Summary));
            }
            if (_printingAppendix.HasFlag(PrintAppendixes.Footer))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Footer));
            }
            if (_printingAppendix.HasFlag(PrintAppendixes.Header))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Header));
            }
        }
    }

    public static class UIElementExtensions
    {

        private static readonly Size MaxSize = new Size(double.MaxValue, double.MaxValue);

        public static Size ComputeDesiredSize(this UIElement uiElement)
        {
            uiElement.Measure(MaxSize);
            return uiElement.DesiredSize;
        }
    }
}
