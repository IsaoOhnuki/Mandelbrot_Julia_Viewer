using Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms;
using Android.Views;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.Droid.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    class DrawPanelRenderer : ViewRenderer<GradationDrawer, SurfaceView>
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
