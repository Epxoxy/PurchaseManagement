using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PurchaseManagement.View
{
    /// <summary>
    /// TextBox with placeholder
    /// </summary>
    public class PlaceholderTextBox : TextBox
    {
        #region Properties
        /// <summary>
        /// Placeholder property
        /// </summary>
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        
        /// <summary>
        /// placeholderForeground property
        /// </summary>
        public Brush PlaceholderForeground
        {
            get { return (Brush)GetValue(PlaceholderForegroundProperty); }
            set { SetValue(PlaceholderForegroundProperty, value); }
        }
        public Brush LostForeground
        {
            get { return (Brush)GetValue(LostForegroundProperty); }
            set { SetValue(LostForegroundProperty, value); }
        }
        public Brush DefaultForeground
        {
            get { return (Brush)GetValue(DefaultForegroundProperty); }
            set { SetValue(DefaultForegroundProperty, value); }
        }
        public CornerRadius lCornerRadius
        {
            get { return (CornerRadius)GetValue(lCornerRadiusProperty); }
            set { SetValue(lCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder", typeof(string), typeof(PlaceholderTextBox),
            new FrameworkPropertyMetadata("Enter here", FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(PlaceholderTextBox), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty LostForegroundProperty =
            DependencyProperty.Register("LostForeground", typeof(Brush), typeof(PlaceholderTextBox), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty DefaultForegroundProperty =
            DependencyProperty.Register("DefaultForeground", typeof(Brush), typeof(PlaceholderTextBox), new PropertyMetadata(Brushes.LightGray));
        public static readonly DependencyProperty lCornerRadiusProperty =
            DependencyProperty.Register("lCornerRadius", typeof(CornerRadius), typeof(PlaceholderTextBox), new PropertyMetadata(new CornerRadius(0, 0, 0, 0)));
        
        #endregion Properties

        #region Constructor

        static PlaceholderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));
        }

        #endregion

        #region Override

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            PlaceholderForeground = (string.IsNullOrEmpty(Text)) ? DefaultForeground : Background;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (string.IsNullOrEmpty(Text)) PlaceholderForeground = LostForeground;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (string.IsNullOrEmpty(Text)) PlaceholderForeground = DefaultForeground;
        }

        #endregion

        #region Recyling

        //private void initializeClearButton()
        //{
        //    var pathFigure = new PathFigure() { StartPoint = new Point(0, 1) };
        //    var polyLineSegment = new PolyLineSegment();
        //    polyLineSegment.Points.Add(new Point(4, 5));
        //    polyLineSegment.Points.Add(new Point(0, 9));
        //    polyLineSegment.Points.Add(new Point(1, 10));
        //    polyLineSegment.Points.Add(new Point(5, 6));
        //    polyLineSegment.Points.Add(new Point(9, 10));
        //    polyLineSegment.Points.Add(new Point(10, 9));
        //    polyLineSegment.Points.Add(new Point(6, 5));
        //    polyLineSegment.Points.Add(new Point(10, 1));
        //    polyLineSegment.Points.Add(new Point(9, 0));
        //    polyLineSegment.Points.Add(new Point(5, 4));
        //    polyLineSegment.Points.Add(new Point(1, 0));
        //    pathFigure.Segments.Add(polyLineSegment);
        //    var pathGeometry = new PathGeometry();
        //    pathGeometry.Figures.Add(pathFigure);
        //    clearButton.Content = new System.Windows.Shapes.Path() { Data = pathGeometry };
        //    clearButton.BorderThickness = new Thickness(0);
        //    clearButton.Background = Brushes.Transparent;
        //    //this.vi
        //}

        #region Events Handling

        ///// <summary>
        ///// Handler for text changed
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void PlaceholderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    Background = string.IsNullOrEmpty(Text) ? _placeholderVisualBrush : null;
        //}
        ///// <summary>
        ///// Handler for lost focus
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void LostFocusHandler(object sender, RoutedEventArgs e)
        //{
        //    _placeholderTextBlock.Foreground = lostForeground;
        //}
        ///// <summary>
        ///// Handler for got focus
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void GotFocusHandler(object sender, RoutedEventArgs e)
        //{
        //    _placeholderTextBlock.Foreground = defaultForeground;
        //}

        #endregion Events Handling

        #endregion
    }
}
