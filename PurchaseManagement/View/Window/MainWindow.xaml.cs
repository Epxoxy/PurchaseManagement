using MySql.Data.MySqlClient;
using PurchaseManagement.Extension;
using PurchaseManagement.View;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace PurchaseManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HomePage homePage;
        private LoginPage loginPage;

        public MainWindow()
        {
            showLoginPage();
            InitializeComponent();
            this.Closing += OnWindowClosing;
        }
        
        #region LoginPage/HomePage Switch

        private void showLoginPage()
        {
            loginPage = new LoginPage();
            loginPage.LoginRequestHander += new EventHandler<SubmitEventArgs>(OnLoginRequest);
            this.AddChild(loginPage);
        }

        private void showHomePage()
        {
            homePage = new HomePage();
            homePage.ExitRequestHander += new EventHandler<SubmitEventArgs>(OnExitHomeRequest);
            this.AddChild(homePage);
        }
        
        private void clearHomePage()
        {
            if (homePage != null)
            {
                homePage.ExitRequestHander -= OnExitHomeRequest;
                homePage.Dispose();
                homePage = null;
                this.Content = null;
            }
        }

        private void clearLoginPage()
        {
            if (loginPage != null)
            {
                loginPage.LoginRequestHander -= OnLoginRequest;
                loginPage.Dispose();
                loginPage = null;
                this.Content = null;
            }
        }

        #endregion

        private void OnLoginRequest(object sender, SubmitEventArgs e)
        {
            object[] requestParams = e.HandValues;
            if (requestParams.Length != 2) return;

            string account = requestParams[0].ToString();
            string password = requestParams[1].ToString();
            if (!string.IsNullOrEmpty(account) && MySqlConnector.loginWith(account, password))
            {
                clearLoginPage();
                showHomePage();
            }
        }
        
        private void OnExitHomeRequest(object sender, SubmitEventArgs e)
        {
            if(e.Type == SubmitType.Logout || e.Type == SubmitType.Error)
            {
                if(e.Type == SubmitType.Error)
                {
                    Alert.show("Module initilize fail! Please contact administrator and check your permission");
                }
                MySqlConnector.logout();
                clearHomePage();
                showLoginPage();
            }
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= OnWindowClosing;
            clearHomePage();
            clearLoginPage();
            base.OnClosing(e);
        }
    }
}
