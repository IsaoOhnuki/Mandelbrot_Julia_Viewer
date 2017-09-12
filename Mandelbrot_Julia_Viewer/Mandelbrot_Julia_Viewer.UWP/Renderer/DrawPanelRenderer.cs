using Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using System.ComponentModel;
using Microsoft.Graphics.Canvas;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.UWP.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.UWP
{
    class DrawPanelRenderer : ViewRenderer<DrawPanel, CanvasControl>
    {
        public CanvasBitmap Image { get; set; }

        public double Scale { get; set; } = 1;

        protected override void OnElementChanged(ElementChangedEventArgs<DrawPanel> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new CanvasControl();
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                Control.Draw -= Control_Draw;
                Control.Tapped -= Control_Tapped;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Draw += Control_Draw;
                Control.Tapped += Control_Tapped;
                Control.ManipulationDelta += Control_ManipulationDelta;
                Control.PointerWheelChanged += Control_PointerWheelChanged;
            }
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == DrawPanel.ImageDataProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageWidthProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageHeightProperty.PropertyName)
            {
                if (Element.ImageData.Length == Element.ImageWidth * Element.ImageHeight * 4)
                {
                    Image = CanvasBitmap.CreateFromBytes(Control, Element.ImageData, Element.ImageWidth, Element.ImageHeight, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96);
                    Control.Invalidate();
                }
            }
        }

        private void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(Control).Properties.MouseWheelDelta;
            if (delta > 0)
            {
                Scale *= 0.2 * delta / 120 + 1;
            }
            else
            {
                Scale += 0.2 * delta / 120 * Scale;
            }
            Control.Invalidate();
        }

        private void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            Scale = Math.Max(e.Delta.Translation.X, e.Delta.Translation.Y);
            Control.Invalidate();
        }

        private void Control_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.GetPosition(Control);
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            double destWidth = (double)Element.ImageWidth * Scale;
            double destHeight = (double)Element.ImageHeight * Scale;
            if (destWidth < Control.ActualWidth || destWidth < destHeight)
            {
                destWidth = Control.ActualWidth;
                Scale = Control.ActualWidth / Element.ImageWidth;
                destHeight = Element.ImageHeight * Scale;
            }
            if (destHeight < Control.ActualHeight || destHeight < destWidth)
            {
                destHeight = Control.ActualHeight;
                Scale = Control.ActualHeight / Element.ImageHeight;
                destWidth = Element.ImageWidth * Scale;
            }
            if (Image != null)
                args.DrawingSession.DrawImage(Image, new Windows.Foundation.Rect(0, 0, destWidth, destHeight), new Windows.Foundation.Rect(0, 0, Element.ImageWidth, Element.ImageHeight));
        }
    }
}
