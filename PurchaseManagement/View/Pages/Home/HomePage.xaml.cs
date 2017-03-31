using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PurchaseManagement.Extension;
using System.Windows.Data;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page, Interfaces.IUIElement
    {
        #region Contructor

        public HomePage()
        {
            InitializeComponent();
            this.Loaded += onLoaded;
        }
        
        ~HomePage()
        {
            Dispose(false);
        }

        #endregion

        #region Build MainMenu
        
        private void buildMainMenu()
        {
            var purchaseList = new BindingList<NavMenus>();
            var infoList = new BindingList<NavMenus>();

            List<NavMenus> menuList;
            NavModuleBuilder.generateNavCmdItems(out menuList);
            if (menuList == null || menuList.Count < 1)
            {
                ExitRequestHander?.Invoke(this, new SubmitEventArgs(SubmitType.Error));
                return;
            };

            var purchaseNavs = new List<NavMenuItem>();
            var infoNavs = new List<NavMenuItem>();
            foreach (var menu in menuList)
            {
                switch (menu.Group)
                {
                    case "purchaseList": { purchaseList.Add(menu); purchaseNavs.AddRange(menu.SubMenus); } break;
                    case "infoList": { infoList.Add(menu); infoNavs.AddRange(menu.SubMenus); } break;
                    default:break;
                }
            }
            if (purchaseNavs.Count < 1) tabControl.Items.Remove(purchaseTabItem);
            else setListBoxItemsSource(purchaseNavs, purchaseListBox);

            if (infoNavs.Count < 1) tabControl.Items.Remove(infoTabItem);
            else setListBoxItemsSource(infoNavs, infoListBox);
        }

        private void setListBoxItemsSource(object source, ListBox target)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(source);
            view.GroupDescriptions.Add(new PropertyGroupDescription() { Converter = Converters.GroupConverter });
            target.ItemsSource = view;
        }
        
        #endregion

        #region Event handler

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            EventsRegistion();
            buildMainMenu();
        }

        private void onLogoutBtnClick(object sender, RoutedEventArgs e)
        {
            ExitRequestHander?.Invoke(sender, new SubmitEventArgs(SubmitType.Logout));
        }

        private void onOkChangePswBtnClick(object sender, RoutedEventArgs e)
        {

            if (newPswBox.Password == confirmPswBox.Password)
            {
                if (MySqlConnector.checkAndUpdatePsw(oldPswBox.Password, confirmPswBox.Password))
                {
                    ExitRequestHander?.Invoke(sender, new SubmitEventArgs(SubmitType.Logout));
                }
                else
                {
                    Alert.show("Update fail, please check you password");
                }
            }
            else
            {
                Alert.show("Please confirm your password");
            }
        }

        public EventHandler<SubmitEventArgs> ExitRequestHander;

        #endregion

        #region Implement IUIElement

        public void EventsRegistion()
        {
            logoutBtn.Click += onLogoutBtnClick;
            okChangePswBtn.Click += onOkChangePswBtnClick;
        }
        
        public void EventsDeregistration()
        {
            this.Loaded -= onLoaded;
            logoutBtn.Click -= onLogoutBtnClick;
            okChangePswBtn.Click -= onOkChangePswBtnClick;
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        #endregion

        #endregion
    }
}
