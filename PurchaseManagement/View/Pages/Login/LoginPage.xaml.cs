using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page, Interfaces.IUIElement
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            EventsRegistion();
            accountBox.ItemsSource = Properties.Settings.Default.HistoryUser;
        }

        public EventHandler<SubmitEventArgs> LoginRequestHander;

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            string userName = accountBox.Text;
            if (Properties.Settings.Default.HistoryUser == null)
            {
                Properties.Settings.Default.HistoryUser = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }
            if(logAccountIDCheckBox.IsChecked == true)
            {
                if (!Properties.Settings.Default.HistoryUser.Contains(userName))
                {
                    Properties.Settings.Default.HistoryUser.Add(userName);
                    Properties.Settings.Default.Save();
                }
            }

            LoginRequestHander?.Invoke(sender, new SubmitEventArgs(SubmitType.Login, userName, passwordBox.Password));
        }

        private void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                LoginButtonClick(sender, e);
                e.Handled = true;
            }
        }

        public void EventsRegistion()
        {
            loginLayer.PreviewKeyDown += OnPreviewKeyDown;
            loginBtn.Click += LoginButtonClick;
        }

        public void EventsDeregistration()
        {
            loginLayer.PreviewKeyDown -= OnPreviewKeyDown;
            loginBtn.Click -= LoginButtonClick;
        }

        ~LoginPage()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }
                // Release unmanaged resources owned by (just) this object.
                EventsDeregistration();
            }
            disposed = true;
        }

        private bool disposed;

        private void langBtn_Click(object sender, RoutedEventArgs e)
        {
            string tips = string.Empty;
            if (Properties.Settings.Default.NewestLang == "zh-CN")
            {
                Properties.Settings.Default.NewestLang = "en-US";
                Properties.Settings.Default.NextLang = "中文";
                tips = Properties.Resources.ChineseToEnglishTips;
            }
            else
            {
                Properties.Settings.Default.NewestLang = "zh-CN";
                Properties.Settings.Default.NextLang = "English";
                tips = Properties.Resources.EnglishToChineseTips;
            }
            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.NewestLang != Properties.Settings.Default.UsingLang)
            {
                if (Extension.Alert.show(tips, System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    App.Restart();
                }
            }
        }

        private void rmAccountClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var param = btn.CommandParameter;
            if(param != null)
            {
                string account = param.ToString();
                if (Properties.Settings.Default.HistoryUser.Contains(account))
                {
                    Properties.Settings.Default.HistoryUser.Remove(account);
                    Properties.Settings.Default.Save();
                    System.Diagnostics.Debug.WriteLine(account);
                    accountBox.ItemsSource = null;
                    accountBox.ItemsSource = Properties.Settings.Default.HistoryUser;
                }
            }
        }
    }

}
