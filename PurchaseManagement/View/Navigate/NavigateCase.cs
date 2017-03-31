using PurchaseManagement.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PurchaseManagement.View
{
    public class NavigateCase
    {
        private static Dictionary<string, Uri> navigateCaseDictionary;
        private static Frame ShowedFrame { get; set; }

        #region Static Command

        //NavigateCommmand for navigate button
        public static DeleagateCommand navigateCommand;
        public static DeleagateCommand NavCmdItemCommand
        {
            get
            {
                if(navigateCommand == null)
                {
                    navigateCommand = new DeleagateCommand()
                    {
                        ExecuteCommand = o =>
                        {
                            NavMenuItem item = o as NavMenuItem;
                            if (item == null) return;
                            navigate(item.CaseString, item.ExtraData);
                        }
                    };
                }
                return navigateCommand; }
        }
        
        public static DeleagateCommand printCommand;
        public static DeleagateCommand PrintCommand
        {
            get
            {
                if (printCommand == null)
                {
                    printCommand = new DeleagateCommand()
                    {
                        ExecuteCommand = o =>
                        {
                            var printContainer = ShowedFrame.Content as Interfaces.ITablePrint;
                            if (printContainer != null)
                            {
                                var printTable = printContainer.GetPrintTable();
                                if (printTable != null) new PrintWindow(printTable) { Owner = Application.Current.MainWindow}.ShowDialog();
                            }
                        }
                    };
                }
                return printCommand;
            }
        }

        #endregion

        #region Property

        public static bool GetRegisterCase(DependencyObject obj)
        {
            return (bool)obj.GetValue(RegisterCaseProperty);
        }
        public static void SetRegisterCase(DependencyObject obj, bool value)
        {
            obj.SetValue(RegisterCaseProperty, value);
        }
        
        public static readonly DependencyProperty RegisterCaseProperty =
            DependencyProperty.RegisterAttached("RegisterCase", typeof(bool), typeof(NavigateCase), new PropertyMetadata(false, OnRegisterChanged));
        
        public static void OnRegisterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frame = d as Frame;
            if(frame != null)
            {
                if ((bool)e.NewValue)
                {
                    frame.IsVisibleChanged += OnFrameVisibleChanged;
                    frame.Navigated += OnFrameNavigated;
                }
                else
                {
                    frame.IsVisibleChanged -= OnFrameVisibleChanged;
                    frame.Navigated -= OnFrameNavigated;
                }
            }
        }

        private static void OnFrameVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null && (bool)e.NewValue) ShowedFrame = frame;
            System.Diagnostics.Debug.WriteLine("OnFrameVisibleChanged running, "+frame);
        }

        private static void OnFrameNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.ExtraData == null) return;
            var transtiveObj = e.Content as Interfaces.ITransitive;
            if (transtiveObj != null)
            {
                transtiveObj.Transmit(e.ExtraData);
            }
            System.Diagnostics.Debug.WriteLine("OnFrameNavigated running");
        }

        #endregion

        public static void initilize()
        {
            navigateCaseDictionary = new Dictionary<string, Uri>();
        }

        public static bool register(string caseString, Uri jumpUri)
        {
            if (string.IsNullOrEmpty(caseString)) return false;
            if (navigateCaseDictionary == null) initilize();
            if (navigateCaseDictionary.ContainsKey(caseString)) navigateCaseDictionary[caseString] = jumpUri;
            else navigateCaseDictionary.Add(caseString, jumpUri);
            return true;
        }

        public static bool unregister(string caseString)
        {
            if (string.IsNullOrEmpty(caseString)) return false;
            if (navigateCaseDictionary.ContainsKey(caseString)) navigateCaseDictionary.Remove(caseString);
            else return false;
            return true;
        }

        public static bool navigate(string caseString, object extradata = null)
        {
            if (ShowedFrame == null || string.IsNullOrEmpty(caseString)) return false;
            if (navigateCaseDictionary.ContainsKey(caseString))
            {
                ShowedFrame.Navigate(navigateCaseDictionary[caseString], extradata);
                return true;
            }
            return false;
        }
        
    }
}
