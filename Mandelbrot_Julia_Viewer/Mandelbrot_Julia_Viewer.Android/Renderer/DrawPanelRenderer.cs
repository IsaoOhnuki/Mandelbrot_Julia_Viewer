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
    // http://serenegiant.com/blog/?p=15 ImageViewを作ってみた〜その１〜準備編

    class DrawPanelRenderer : ViewRenderer<GradationDrawer, Android.Views.View>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GradationDrawer> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new Android.Views.View(this.Context);

                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                Control.ContextClick -= Control_ContextClick;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.ContextClick += Control_ContextClick;
            }
            base.OnElementChanged(e);
        }

        private void Control_ContextClick(object sender, ContextClickEventArgs e)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}
