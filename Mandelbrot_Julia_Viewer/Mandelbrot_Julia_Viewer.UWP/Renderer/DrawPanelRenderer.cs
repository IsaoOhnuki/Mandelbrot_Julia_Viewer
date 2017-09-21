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
namespace Mandelbrot_Julia_Viewer.UWP
{
    // https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/BasicInput Basic input sample

    class DrawPanelRenderer : ViewRenderer<DrawPanel, CanvasControl>
    {
        public Size ViewSize { get { return new Size(Control.ActualWidth, Control.ActualHeight); } }
        public Point ViewOrigin { get { return new Point(Control.ActualWidth / 2, Control.ActualHeight / 2); } }

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
                e.OldElement.ImageCompaile -= ImageCompaile;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Draw += Control_Draw;
                Control.SizeChanged += Control_SizeChanged;
                Control.ManipulationDelta += Control_ManipulationDelta;
                Control.PointerWheelChanged += Control_PointerWheelChanged;
                e.NewElement.ImageCompaile += ImageCompaile;
            }
            base.OnElementChanged(e);
        }

        Task<object> ImageCompaile(byte[] image, int x, int y, int p)
        {
            return Task.Run(() => {
                return (object)CanvasBitmap.CreateFromBytes(CanvasDevice.GetSharedDevice(), image, x, y, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96);
            });
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        private void Control_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            //Debug.WriteLine("Control_SizeChanged");
            Element.ViewRect = new Rectangle(new Point(-ViewOrigin.X * Element.ViewScale, -ViewOrigin.Y * Element.ViewScale), ViewSize * Element.ViewScale);
            Control.Invalidate();
        }

        private void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_PointerWheelChanged");
            var pointer = e.GetCurrentPoint(Control).Properties;
            if (!pointer.IsLeftButtonPressed && !pointer.IsRightButtonPressed)
            {
                //int delta = pointer.MouseWheelDelta;
                //if (delta > 0)
                //{
                //    Scale = new ScalingStruct { Scale = Scale.Scale * (0.2 * delta / 120 + 1), Position = new Point(pointer.ContactRect.X - ViewOrigin.X, pointer.ContactRect.Y - ViewOrigin.Y) };
                //}
                //else
                //{
                //    Scale = new ScalingStruct { Scale = Scale.Scale + 0.2 * delta / 120 * Scale.Scale, Position = new Point(pointer.ContactRect.X - ViewOrigin.X, pointer.ContactRect.Y - ViewOrigin.Y) };
                //}
                //if (DrawImageValid)
                //    Control.Invalidate();
            }
        }

        private void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_ManipulationDelta");
            //Origin = new Point(Origin.X + e.Delta.Translation.X, Origin.Y + e.Delta.Translation.Y);
            //if (DrawImageValid)
            //    Control.Invalidate();
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            //args.DrawingSession.FillRectangle(new Windows.Foundation.Rect(0, 0, Control.ActualWidth, Control.ActualHeight), Windows.UI.Color.FromArgb((byte)(Element.BackgroundColor.A * 255), (byte)(Element.BackgroundColor.R), (byte)(Element.BackgroundColor.G), (byte)(Element.BackgroundColor.B)));
            DeviceImageStruct deviceImage = Element.GetDeviceImage();
            if (deviceImage != null && deviceImage.Image != null)
                args.DrawingSession.DrawImage((ICanvasImage)deviceImage.Image, new Windows.Foundation.Rect(deviceImage.ViewRect.X, deviceImage.ViewRect.Y, deviceImage.ViewRect.Width, deviceImage.ViewRect.Height), new Windows.Foundation.Rect(deviceImage.DrawRect.X, deviceImage.DrawRect.Y, deviceImage.DrawRect.Width, deviceImage.DrawRect.Height));
        }
    }
}
