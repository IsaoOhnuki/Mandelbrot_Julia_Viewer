using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ColorPicker), typeof(Mandelbrot_Julia_Viewer.Droid.ColorPickerRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    class ColorPickerRenderer : ViewRenderer<ColorPicker, Spinner>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ColorPicker> e)
        {
            var ctrl = new Spinner()
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}