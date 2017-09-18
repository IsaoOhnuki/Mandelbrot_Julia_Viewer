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

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.UWP.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.UWP
{
    // https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/BasicInput Basic input sample

    class DrawPanelRenderer : ViewRenderer<DrawPanel, CanvasControl>
    {
        private DrawPanel.DrawImageStruct drawImage;
        public DrawPanel.DrawImageStruct DrawImage
        {
            get { return drawImage; }
            set
            {
                drawImage = value;
                drawRect.X = Origin.X + ViewOrigin.X - DrawImage.ImageSizeX / 2;
                drawRect.Y = Origin.Y + ViewOrigin.Y - DrawImage.ImageSizeY / 2;
                drawRect.Width = DrawImage.ImageSizeX * Scale.Scale;
                drawRect.Height = DrawImage.ImageSizeY * Scale.Scale;
            }
        }
        public bool DrawImageValid { get { return DrawImage != null && DrawImage.Image.Length == DrawImage.ImageSizeX * DrawImage.ImageSizeY * 4; } }
        public CanvasBitmap Image { get; set; }

        private Windows.Foundation.Rect drawRect;
        public Windows.Foundation.Rect DrawRect { get { return drawRect; } }

        public Point ViewOrigin
        {
            get { return new Point(Control.ActualWidth / 2, Control.ActualHeight / 2); }
        }

        private Point origin;
        public Point Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                drawRect.X = Origin.X + ViewOrigin.X - DrawImage.ImageSizeX / 2;
                drawRect.Y = Origin.Y + ViewOrigin.Y - DrawImage.ImageSizeY / 2;
            }
        }

        public struct ScalingStruct
        {
            public double Scale;
            public Point Position;
        }

        private ScalingStruct scale = new ScalingStruct { Scale = 1 };
        public ScalingStruct Scale
        {
            get { return scale; }
            set
            {
                double oldScale = Scale.Scale;

                scale = value;
                if (scale.Scale < 0.001)
                    scale.Scale = 0.001;

                double width = DrawImage.ImageSizeX * Scale.Scale;
                double height = DrawImage.ImageSizeY * Scale.Scale;

                double distanceX = Scale.Position.X * Scale.Scale;
                double distanceY = Scale.Position.Y * Scale.Scale;

                //Origin = new Point(Origin.X + (drawRect.Width - width) / 2 + distanceX, Origin.Y + (drawRect.Height - height) / 2 + distanceY);
                Origin = new Point(Origin.X + (drawRect.Width - width) / 2, Origin.Y + (drawRect.Height - height) / 2);

                drawRect.Width = width;
                drawRect.Height = height;
            }
        }

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
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Draw += Control_Draw;
                Control.SizeChanged += Control_SizeChanged;
                Control.ManipulationDelta += Control_ManipulationDelta;
                Control.PointerWheelChanged += Control_PointerWheelChanged;
            }
            base.OnElementChanged(e);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "DrawImage")
            {
                DrawImage = Element.DrawImage;
                Image = await Task<CanvasBitmap>.Run(() =>
                {
                    var dvc = CanvasDevice.GetSharedDevice();
                    return CanvasBitmap.CreateFromBytes(dvc, DrawImage.Image, DrawImage.ImageSizeX, DrawImage.ImageSizeY, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96);
                });
                Control.Invalidate();
            }
        }

        private void Control_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            //Debug.WriteLine("Control_SizeChanged");
            if (DrawImageValid)
            {
                Origin = Origin;
                Control.Invalidate();
            }
        }

        private void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_PointerWheelChanged");
            var pointer = e.GetCurrentPoint(Control).Properties;
            if (!pointer.IsLeftButtonPressed && !pointer.IsRightButtonPressed)
            {
                int delta = pointer.MouseWheelDelta;
                if (delta > 0)
                {
                    Scale = new ScalingStruct { Scale = Scale.Scale * (0.2 * delta / 120 + 1), Position = new Point(pointer.ContactRect.X - ViewOrigin.X, pointer.ContactRect.Y - ViewOrigin.Y) };
                }
                else
                {
                    Scale = new ScalingStruct { Scale = Scale.Scale + 0.2 * delta / 120 * Scale.Scale, Position = new Point(pointer.ContactRect.X - ViewOrigin.X, pointer.ContactRect.Y - ViewOrigin.Y) };
                }
                if (DrawImageValid)
                    Control.Invalidate();
            }
        }

        private void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            //Debug.WriteLine("Control_ManipulationDelta");
            Origin = new Point(Origin.X + e.Delta.Translation.X, Origin.Y + e.Delta.Translation.Y);
            if (DrawImageValid)
                Control.Invalidate();
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Image != null)
            {
                args.DrawingSession.FillRectangle(new Windows.Foundation.Rect(0, 0, Control.ActualWidth, Control.ActualHeight), Windows.UI.Color.FromArgb((byte)(Element.BackgroundColor.A * 255), (byte)(Element.BackgroundColor.R), (byte)(Element.BackgroundColor.G), (byte)(Element.BackgroundColor.B)));
                args.DrawingSession.DrawImage(Image, DrawRect);
            }
        }
    }
}
