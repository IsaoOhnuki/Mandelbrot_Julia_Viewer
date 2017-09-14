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

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.UWP.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.UWP
{
    class DrawPanelRenderer : ViewRenderer<DrawPanel, CanvasControl>
    {
        public CanvasBitmap Image { get; set; }
        private Point origin;
        public Point Origin
        {
            get { return origin; }
            set
            {
                origin = value;
            }
        }
        private double scale = 1;
        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
            }
        }

        public async Task<bool> UpdateImageASync()
        {
            if (Element.ImageDataValid)
            {
                DrawPanel.DrawImage image = await Task<DrawPanel.DrawImage>.Factory.StartNew(
                    sz => Element.GetDrawImage(Origin, Scale, (Size)sz, false),
                    new Size(Control.ActualWidth / 2, Control.ActualHeight / 2));
                Scale = image.Scale;
                Origin = image.Origin;
                Image = await Task.Run<CanvasBitmap>(() => CanvasBitmap.CreateFromBytes(Control, Element.ImageData, image.ImageSizeX, image.ImageSizeY, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96));
                return true;
            }
            return false;
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
                Control.Tapped -= Control_Tapped;
                Control.ManipulationDelta -= Control_ManipulationDelta;
                Control.PointerWheelChanged -= Control_PointerWheelChanged;
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

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == DrawPanel.ImageDataProperty.PropertyName
                || e.PropertyName == DrawPanel.ImagePixelOfByteSizeProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageOriginProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageWidthProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageHeightProperty.PropertyName)
            {
                if (await UpdateImageASync())
                    Control.Invalidate();
            }
        }

        private async void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(Control).Properties.MouseWheelDelta;
            double oldScale = Scale;
            if (delta > 0)
            {
                Scale *= 0.2 * delta / 120 + 1;
            }
            else
            {
                Scale += 0.2 * delta / 120 * Scale;
            }
            if (await UpdateImageASync())
                Control.Invalidate();
        }

        private async void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            Scale *= e.Delta.Scale;
            if (await UpdateImageASync())
                Control.Invalidate();
        }

        private void Control_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.GetPosition(Control);
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Image != null)
            {
                args.DrawingSession.DrawImage(Image);
            }
        }
    }
}
