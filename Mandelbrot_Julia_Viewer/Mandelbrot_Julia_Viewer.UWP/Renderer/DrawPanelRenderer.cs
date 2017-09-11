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
        enum InvalidType
        {
            None,
            Image,
        }
        InvalidType invalidType { get; set; }

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
                e.OldElement.ImageChanged -= Element_ImageChanged;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Draw += Control_Draw;
                Control.Tapped += Control_Tapped;
                e.NewElement.ImageChanged += Element_ImageChanged;
            }
            base.OnElementChanged(e);
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            switch (invalidType)
            {
                case InvalidType.Image:
                    if (Element.Image is byte[])
                    {
                        CanvasBitmap image = CanvasBitmap.CreateFromBytes(Control, Element.Image as byte[], 4096, 4096, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized);
                        args.DrawingSession.DrawImage(image);
                        //using (InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream())
                        //{
                        //    await randomAccessStream.WriteAsync((Element.Image as byte[]).AsBuffer());
                        //    CanvasBitmap image = await CanvasBitmap.LoadAsync(Control, randomAccessStream);
                        //    args.DrawingSession.DrawImage(image);
                        //}
                    }
                    break;
            }
        }

        private void Element_ImageChanged(object sender, ImageChangedEventArgs e)
        {
            invalidType = InvalidType.Image;
            Control.Invalidate();
        }

        private void Control_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.GetPosition(Control);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}
