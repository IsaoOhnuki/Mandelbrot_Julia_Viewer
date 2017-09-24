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
        public Point ViewPoint { get; set; }
        public Size ViewSize { get { return new Size(Control.ActualWidth, Control.ActualHeight); } }
        public Point ViewOrigin { get { return new Point(Control.ActualWidth / 2, Control.ActualHeight / 2); } }
        private double scale = 1;
        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                if (scale < 0.001)
                    scale = 0.001;
            }
        }

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

                Element.ImageCompile -= ImageCompile;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Draw += Control_Draw;
                Control.SizeChanged += Control_SizeChanged;
                Control.ManipulationDelta += Control_ManipulationDelta;
                Control.PointerWheelChanged += Control_PointerWheelChanged;

                Element.ImageCompile += ImageCompile;
            }
            base.OnElementChanged(e);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "DeviceImage")
            {
                DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

                Control.Invalidate();
            }
        }

        Task<object> ImageCompile(byte[] image, int x, int y, int p)
        {
            return Task.Run(() => {
                return (object)CanvasBitmap.CreateFromBytes(CanvasDevice.GetSharedDevice(), image, x, y, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96);
            });
        }

        private async void Control_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            //Debug.WriteLine("Control_SizeChanged");

            DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

            Control.Invalidate();
        }

        private async void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_PointerWheelChanged");
            var pointer = e.GetCurrentPoint(Control).Properties;
            if (!pointer.IsLeftButtonPressed && !pointer.IsRightButtonPressed)
            {
                int delta = pointer.MouseWheelDelta;
                if (delta > 0)
                {
                    Scale = Scale * (0.2 * delta / 120 + 1);
                }
                else if (delta < 0)
                {
                    Scale = Scale / (0.2 * -delta / 120 + 1);
                }

                ViewPoint = ViewPoint.Offset((ViewSize.Width - ViewSize.Width / Scale) / 2, (ViewSize.Height - ViewSize.Height / Scale) / 2);

                DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

                Control.Invalidate();
            }
        }

        private async void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_ManipulationDelta");
            ViewPoint = ViewPoint.Offset(-e.Delta.Translation.X / Scale, -e.Delta.Translation.Y / Scale);

            DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

            Control.Invalidate();
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.FillRectangle(GetDeviceRect(new Rectangle(Point.Zero, ViewSize)), Windows.UI.Color.FromArgb((byte)(Element.BackgroundColor.A * 255d), (byte)(Element.BackgroundColor.R * 255d), (byte)(Element.BackgroundColor.G * 255d), (byte)(Element.BackgroundColor.B * 255d)));

            if (DrawImage != null && DrawImage.Image != null)
            {
                //Rectangle reqRect = new Rectangle(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

                //Rectangle viewRect = DrawImage.DrawRect.Offset(-ViewPoint.X, -ViewPoint.Y);
                Rectangle viewRect = new Rectangle
                {
                    Location = Matrix2.Enlargement(DrawImage.DrawRect.Location.Offset(-ViewPoint.X, -ViewPoint.Y), Scale, Scale),
                    Size = Matrix2.Enlargement(DrawImage.DrawRect.Size, Scale, Scale)
                };

                args.DrawingSession.DrawImage(DrawImage.Image as ICanvasImage, GetDeviceRect(viewRect), GetDeviceRect(DrawImage.DrawRect));
            }
        }

        public Windows.Foundation.Rect GetDeviceRect(Rectangle rect)
        {
            return new Windows.Foundation.Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
