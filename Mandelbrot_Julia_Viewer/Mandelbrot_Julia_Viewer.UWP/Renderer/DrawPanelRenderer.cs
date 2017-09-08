using Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.UWP.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.UWP
{
    class DrawPanelRenderer : ViewRenderer<GradationDrawer, CanvasControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GradationDrawer> e)
        {
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}
