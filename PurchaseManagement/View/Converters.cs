using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PurchaseManagement.View
{
    [ValueConversion(typeof(String), typeof(String))]
    public class ListBoxGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var menus = value as NavMenuItem;
            if (menus == null) return "Undefined";
            if (string.IsNullOrEmpty(menus.GroupName)) return "Undefined";
            return menus.GroupName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class Bool2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool sourceValue = System.Convert.ToBoolean(value);
            if (sourceValue) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility sourceValue = (Visibility)value;
            if (sourceValue == Visibility.Visible) return true;
            else return false;
        }
    }
    
    [ValueConversion(typeof(DataGridHeadersVisibility), typeof(Visibility))]
    public class HeaderVisibility2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            var sourceValue = (DataGridHeadersVisibility)value;
            var parameterValue = (DataGridHeadersVisibility)parameter;

            if (sourceValue == parameterValue || sourceValue == DataGridHeadersVisibility.All) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class TrueToFalse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if(value is bool)
            {
                return !(bool)value;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Type), typeof(bool))]
    public class IsTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            var valueType = value as Type;
            var type = parameter as Type;
            if (valueType == null || type == null) return false;

            if (valueType == type)
            {
                return true;
            }
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class Converters
    {
        private static TrueToFalse trueToFalse;
        public static TrueToFalse TrueToFalse
        {
            get
            {
                if (trueToFalse == null) trueToFalse = new TrueToFalse();
                return trueToFalse;
            }
        }
        
        private static IsTypeConverter isTypeConverter;
        public static IsTypeConverter IsTypeConverter
        {
            get
            {
                if (isTypeConverter == null) isTypeConverter = new IsTypeConverter();
                return isTypeConverter;
            }
        }

        private static ListBoxGroupConverter groupConverter;
        public static ListBoxGroupConverter GroupConverter
        {
            get
            {
                if (groupConverter == null) groupConverter = new ListBoxGroupConverter();
                return groupConverter;
            }
        }

        private static Bool2Visibility bool2Visibility;
        public static Bool2Visibility Bool2Visibility
        {
            get
            {
                if (bool2Visibility == null) bool2Visibility = new Bool2Visibility();
                return bool2Visibility;
            }
        }

        private static HeaderVisibility2Visibility headerVisibility2Visibility;
        public static HeaderVisibility2Visibility HeaderVisibility2Visibility
        {
            get
            {
                if (headerVisibility2Visibility == null) headerVisibility2Visibility = new HeaderVisibility2Visibility();
                return headerVisibility2Visibility;
            }
        }
    }
}
