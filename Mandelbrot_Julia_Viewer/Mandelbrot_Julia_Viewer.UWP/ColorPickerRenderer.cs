using Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ColorPicker), typeof(Mandelbrot_Julia_Viewer.UWP.ColorPickerRenderer))]
namespace Mandelbrot_Julia_Viewer.UWP
{
    // https://docs.microsoft.com/ja-jp/windows/uwp/controls-and-patterns/listview-item-templates 項目コンテナーやテンプレート
    class ColorPickerRenderer : ViewRenderer<ColorPicker, ComboBox>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ColorPicker> e)
        {
            if (Control == null)
            {
                var ctrl = new ComboBox();
                SetNativeControl(ctrl);
                var tmp = new Windows.UI.Xaml.DataTemplate();
                var factory = new FrameworkElementFactory(typeof(Label));
                Control.ItemTemplate = tmp;
            }
            if (e.NewElement != null)
            {
                Control.ItemsSource = e.NewElement.ListItems;
            }
            base.OnElementChanged(e);
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // プロパティ値の変更を反映
            if (e.PropertyName == ColorPicker.SelectedColorProperty.PropertyName)
            {
            }
        }
    }
}
