using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PurchaseManagement.Extension
{
    public class FrameExtension
    {
        public static bool GetCancelJournal(DependencyObject obj)
        {
            return (bool)obj.GetValue(CancelJournalProperty);
        }
        public static void SetCancelJournal(DependencyObject obj, bool value)
        {
            obj.SetValue(CancelJournalProperty, value);
        }
        
        public static readonly DependencyProperty CancelJournalProperty =
            DependencyProperty.RegisterAttached("CancelJournal", typeof(bool), typeof(FrameExtension), new PropertyMetadata(false, OnCancelJournalChanged));

        private static void OnCancelJournalChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var frame = obj as System.Windows.Controls.Frame;
            if(frame != null)
            {
                if ((bool)e.NewValue)
                {
                    frame.NavigationService.Navigating -= OnFrameNavigating;
                    frame.NavigationService.Navigated -= OnFrameNavigated;
                    frame.NavigationService.Navigating += OnFrameNavigating;
                    frame.NavigationService.Navigated += OnFrameNavigated;
                }
                else
                {
                    frame.NavigationService.Navigating -= OnFrameNavigating;
                    frame.NavigationService.Navigated -= OnFrameNavigated;
                }
            }
        }
        
        #region Navigate Event Handler

        private static void OnFrameNavigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back || e.NavigationMode == System.Windows.Navigation.NavigationMode.Forward)
            {
                e.Cancel = true;
            }
        }

        private static void OnFrameNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var frame = sender as System.Windows.Controls.Frame;
            if (frame != null) frame.NavigationService.RemoveBackEntry();
        }

        #endregion

    }
}
