using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PurchaseManagement.Extension
{
    public class BitmapSourceHelper
    {
        public static BitmapSource CreateBitmapSourceFromVisual(FrameworkElement frameworkElement)
        {
            return CreateBitmapSourceFromVisual(frameworkElement.ActualWidth, frameworkElement.ActualHeight, frameworkElement);
        }

        public static BitmapSource CreateBitmapSourceFromVisual(Double width, Double height, Visual visualToRender, Boolean undoTransformation = false)
        {
            if (visualToRender == null)
            {
                return null;
            }
            
            RenderTargetBitmap bmp = new RenderTargetBitmap((Int32)Math.Ceiling(width), (Int32)Math.Ceiling(height), 96, 96, PixelFormats.Pbgra32);
            if (undoTransformation)
            {
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(visualToRender);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                }
                bmp.Render(dv);
            }
            else
            {
                bmp.Render(visualToRender);
            }
            return bmp;
        }
    }
}
