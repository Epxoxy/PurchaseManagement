using PurchaseManagement.Extension;
using System.Windows;
using System.Windows.Controls;

namespace PurchaseManagement.View
{
    /// <summary>
    /// Interaction logic for IntervalBox.xaml
    /// </summary>
    public partial class IntervalBox : UserControl
    {
        public bool UseDateBox
        {
            get { return (bool)GetValue(UseDateBoxProperty); }
            set { SetValue(UseDateBoxProperty, value); }
        }
        public static readonly DependencyProperty UseDateBoxProperty =
            DependencyProperty.Register("UseDateBox", typeof(bool), typeof(IntervalBox), new PropertyMetadata(false, OnUseDateBoxChanged));
        
        public bool DisableToPart
        {
            get { return (bool)GetValue(DisableToPartProperty); }
            set { SetValue(DisableToPartProperty, value); }
        }
        public static readonly DependencyProperty DisableToPartProperty =
            DependencyProperty.Register("DisableToPart", typeof(bool), typeof(IntervalBox), new PropertyMetadata(false, OnDisableToPartChanged));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(IntervalBox), new PropertyMetadata(Properties.Resources.Interval));
        
        public bool UseComboBox
        {
            get { return (bool)GetValue(UseComboBoxProperty); }
            set { SetValue(UseComboBoxProperty, value); }
        }
        public static readonly DependencyProperty UseComboBoxProperty =
            DependencyProperty.Register("UseComboBox", typeof(bool), typeof(IntervalBox), new PropertyMetadata(false, OnUseComboBoxChanged));



        public System.Collections.IEnumerable ItemsSource
        {
            get { return (System.Collections.IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(System.Collections.IEnumerable), typeof(IntervalBox), new PropertyMetadata(null));



        private static void OnUseDateBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as IntervalBox;
            if (box == null) return;
            if (box.UseComboBox) box.UseComboBox = false;
            box.switchDateBoxState((bool)e.NewValue);
        }

        private static void OnDisableToPartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as IntervalBox;
            if (box == null) return;
            box.switchToPartState((bool)e.NewValue);
        }

        private static void OnUseComboBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as IntervalBox;
            if (box == null) return;
            if (box.UseDateBox) box.UseDateBox = false;
            box.switchComboBoxState((bool)e.NewValue);
        }

        public string FromValue
        {
            get
            {
                if (UseComboBox)
                {
                    if (fromComboBox.SelectedIndex < 0) return string.Empty;
                    return fromComboBox.SelectedValue.ToString();
                }
                return UseDateBox ? fromPicker.SelectedDate?.ToString("yyyy-MM-dd HH:mm:ss") : fromTextBox.Text;
            }
        }
        public string ToValue
        {
            get
            {
                if (DisableToPart) return string.Empty;
                if (UseComboBox)
                {
                    if (toComboBox.SelectedIndex < 0) return string.Empty;
                    return toComboBox.SelectedValue.ToString();
                }
                return UseDateBox? toPicker.SelectedDate?.ToString("yyyy-MM-dd HH:mm:ss") : toTextBox.Text;
            }
        }

        public IntervalBox()
        {
            InitializeComponent();
            boxroot.DataContext = this;
        }

        private void switchDateBoxState(bool isNeedUse)
        {
            if (isNeedUse)
            {
                fromTextBox.Visibility = Visibility.Collapsed;
                toTextBox.Visibility = Visibility.Collapsed;
                fromComboBox.Visibility = Visibility.Collapsed;
                toComboBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                fromTextBox.Visibility = Visibility.Visible;
                toTextBox.Visibility = Visibility.Visible;
                fromComboBox.Visibility = Visibility.Visible;
                toComboBox.Visibility = Visibility.Visible;
            }
        }
        private void switchComboBoxState(bool isNeedUse)
        {
            if (isNeedUse)
            {
                fromComboBox.Visibility = Visibility.Visible;
                toComboBox.Visibility = Visibility.Visible;
                fromTextBox.Visibility = Visibility.Collapsed;
                toTextBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                fromComboBox.Visibility = Visibility.Collapsed;
                toComboBox.Visibility = Visibility.Collapsed;
                fromTextBox.Visibility = Visibility.Visible;
                toTextBox.Visibility = Visibility.Visible;
            }
        }
        private void switchToPartState(bool disableUse)
        {
            if (disableUse)
            {
                intervalLine.Visibility = Visibility.Collapsed;
                toPart.Visibility = Visibility.Collapsed;
            }
            else
            {
                intervalLine.Visibility = Visibility.Visible;
                toPart.Visibility = Visibility.Visible;
            }
        }

        public void emptyValue()
        {
            if (UseDateBox)
            {
                fromPicker.SelectedDate = null;
                toPicker.SelectedDate = null;
            }
            else
            {
                fromTextBox.Text = string.Empty;
                toTextBox.Text = string.Empty;
            }
        }
        
        public void getCondition(string columnName, out string condition)
        {
            condition = string.Empty;
            string from = FromValue;
            string to = ToValue;
            bool isEmptyFrom = string.IsNullOrEmpty(from), isEmptyTo = string.IsNullOrEmpty(to);
            if (DisableToPart)
            {
                if (!isEmptyFrom) condition = string.Format(" {0} = '{1}' ", columnName, from);
                return;
            }
            //Start combine condition sentences
            if (!isEmptyFrom)
            {
                if (isEmptyTo) condition = string.Format(" {0} >= '{1}' ", columnName, from);
                else
                {
                    if (from == to) condition = string.Format(" {0} = '{1}' ", columnName, from);
                    else condition = string.Format(" {0} >= '{1}' and {0} <= '{2}' ", columnName, from, to);
                }
            }
            else
            {
                if (!isEmptyTo) condition = string.Format(" {0} <= '{1}' ", columnName, to);
            }
        }
    }
}
