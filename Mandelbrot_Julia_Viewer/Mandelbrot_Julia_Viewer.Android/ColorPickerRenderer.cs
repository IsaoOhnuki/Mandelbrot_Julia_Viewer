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
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new Spinner(this.Context);

                //var tmp = Application.Current.Resources["ColorPickerItemTemplate"];
                //var tmp = Windows.UI.Xaml.Markup.XamlReader.Load(
                //    @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                //        <!--<Grid Background=""{Binding Color16Str}"">
                //            <TextBlock Text=""{Binding Name}""/>
                //        </Grid>-->
                //        <Grid CornerRadius=""5"">
                //            <Grid.Background>
                //                <LinearGradientBrush EndPoint=""0.6, 0.5"" StartPoint=""0, 0.5"">
                //                    <GradientStop Color=""#FFFFFFFF""/>
                //                    <GradientStop Color=""{Binding Color16Str}"" Offset=""1""/>
                //                </LinearGradientBrush>
                //            </Grid.Background>
                //            <TextBlock Text=""{Binding Name}"" Padding=""5""/>
                //        </Grid>
                //    </DataTemplate>");

                //ctrl.ItemTemplate = tmp as Windows.UI.Xaml.DataTemplate;
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                Control.ItemSelected -= Control_ItemSelected;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Adapter = new ArrayAdapter<ColorStruct>(this.Context, 0, e.NewElement.ListItems);
                Control.SetSelection(e.NewElement.ListItems.IndexOf((e.NewElement.ListItems as IEnumerable<ColorStruct>)?.Where(x => x.Color == e.NewElement.SelectedColor).SingleOrDefault()));
                Control.ItemSelected += Control_ItemSelected;
            }
            base.OnElementChanged(e);
        }

        private void Control_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Element.SelectedColor = Element.ListItems[e.Position].Color;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}