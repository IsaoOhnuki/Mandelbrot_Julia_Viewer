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
using Xamarin.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using static Controls.DrawPanel;

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.UWP.DrawPanelRenderer))]
[assembly: Dependency (typeof(Mandelbrot_Julia_Viewer.UWP.DeviceBitmapCreator))]

namespace Mandelbrot_Julia_Viewer.UWP
{
    // http://www.buildinsider.net/mobile/xamarintips/0008 Xamarin.Formsからプラットフォーム固有の機能を利用するには？（DependencyService利用）
    // https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/BasicInput Basic input sample

    class DrawPanelRenderer : ViewRenderer<DrawPanel, CanvasControl>
    {
        public Size ViewSize { get { return new Size(Control.ActualWidth, Control.ActualHeight); } }
        public Point ViewOrigin { get { return new Point(Control.ActualWidth / 2, Control.ActualHeight / 2); } }

        public DrawImageStruct DrawImage { get; set; }

        protected override void OnElementChanged(ElementChangedEventArgs<DrawPanel> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new CanvasControl();
                ctrl.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All;
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                Control.Draw -= Control_Draw;
                Control.SizeChanged -= Control_SizeChanged;
                Control.ManipulationDelta -= Control_ManipulationDelta;
                Control.PointerWheelChanged -= Control_PointerWheelChanged;

                Control.Tapped -= Control_Tapped;
                Control.DoubleTapped -= Control_DoubleTapped;
                Control.Holding -= Control_Holding;
                Control.RightTapped -= Control_RightTapped;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Draw += Control_Draw;
                Control.SizeChanged += Control_SizeChanged;
                Control.ManipulationDelta += Control_ManipulationDelta;
                Control.PointerWheelChanged += Control_PointerWheelChanged;

                Control.Tapped += Control_Tapped;
                Control.DoubleTapped += Control_DoubleTapped;
                Control.Holding += Control_Holding;
                Control.RightTapped += Control_RightTapped;
            }
            base.OnElementChanged(e);
        }

        private void Control_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var pos = e.GetPosition((UIElement)sender);
            Element.OnLongTapped(pos.X / Element.ViewScale + Element.ViewPoint.X, pos.Y / Element.ViewScale + Element.ViewPoint.Y);
        }

        private void Control_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var pos = e.GetPosition((UIElement)sender);
            Element.OnLongTapped(pos.X / Element.ViewScale + Element.ViewPoint.X, pos.Y / Element.ViewScale + Element.ViewPoint.Y);
        }

        private void Control_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var pos = e.GetPosition((UIElement)sender);
            Element.OnDoubleTapped(pos.X / Element.ViewScale + Element.ViewPoint.X, pos.Y / Element.ViewScale + Element.ViewPoint.Y);
        }

        private void Control_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pos = e.GetPosition((UIElement)sender);
            Element.OnTapped(pos.X / Element.ViewScale + Element.ViewPoint.X, pos.Y / Element.ViewScale + Element.ViewPoint.Y);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "DeviceImage")
            {
                DrawImage = await Element.DrawImmageRequestAsync(Element.ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Element.ViewScale, 1 / Element.ViewScale));

                Control.Invalidate();
            }
        }

        private async void Control_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            //Debug.WriteLine("Control_SizeChanged");

            DrawImage = await Element.DrawImmageRequestAsync(Element.ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Element.ViewScale, 1 / Element.ViewScale));

            Control.Invalidate();
        }

        private async void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_PointerWheelChanged");
            var pointer = e.GetCurrentPoint(Control).Properties;
            if (!pointer.IsLeftButtonPressed && !pointer.IsRightButtonPressed)
            {
                double oldScale = Element.ViewScale;
                int delta = pointer.MouseWheelDelta;
                if (delta > 0)
                {
                    Element.ViewScale = Element.ViewScale * (0.2 * delta / 120 + 1);
                }
                else if (delta < 0)
                {
                    Element.ViewScale = Element.ViewScale / (0.2 * -delta / 120 + 1);
                }

                Point oldPoint = Element.ViewPoint.Offset(pointer.ContactRect.X / oldScale, pointer.ContactRect.Y / oldScale);
                Point newPoint = Element.ViewPoint.Offset(pointer.ContactRect.X / Element.ViewScale, pointer.ContactRect.Y / Element.ViewScale);
                Element.ViewPoint = Element.ViewPoint.Offset(oldPoint.X - newPoint.X, oldPoint.Y - newPoint.Y);

                DrawImage = await Element.DrawImmageRequestAsync(Element.ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Element.ViewScale, 1 / Element.ViewScale));

                Control.Invalidate();
            }
        }

        private async void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_ManipulationDelta");

            Element.ViewPoint = Element.ViewPoint.Offset(-e.Delta.Translation.X / Element.ViewScale, -e.Delta.Translation.Y / Element.ViewScale);
            
            DrawImage = await Element.DrawImmageRequestAsync(Element.ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Element.ViewScale, 1 / Element.ViewScale));

            Control.Invalidate();
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.FillRectangle(GetDeviceRect(new Rectangle(Point.Zero, ViewSize)), Windows.UI.Color.FromArgb((byte)(Element.BackgroundColor.A * 255d), (byte)(Element.BackgroundColor.R * 255d), (byte)(Element.BackgroundColor.G * 255d), (byte)(Element.BackgroundColor.B * 255d)));

            if (DrawImage != null && DrawImage.Image != null)
            {
                Rectangle viewRect = new Rectangle
                {
                    Location = Matrix2.Enlargement(DrawImage.DrawRect.Location.Offset(-Element.ViewPoint.X, -Element.ViewPoint.Y), Element.ViewScale, Element.ViewScale),
                    Size = Matrix2.Enlargement(DrawImage.DrawRect.Size, Element.ViewScale, Element.ViewScale)
                };

                args.DrawingSession.DrawImage(DrawImage.Image as ICanvasImage, GetDeviceRect(viewRect), GetDeviceRect(DrawImage.DrawRect), 1f, CanvasImageInterpolation.HighQualityCubic);
            }
        }

        public Windows.Foundation.Rect GetDeviceRect(Rectangle rect)
        {
            return new Windows.Foundation.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }

    public class DeviceBitmapCreator : IDeviceBitmapCreator
    {
        public Task<object> Create(byte[] imageData, int imageWidth, int imageHeight, int pixelByteCount)
        {
            return Task.Run(() => {
                return (object)CanvasBitmap.CreateFromBytes(CanvasDevice.GetSharedDevice(), imageData, imageWidth, imageHeight, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96);
            });
        }
    }
}
