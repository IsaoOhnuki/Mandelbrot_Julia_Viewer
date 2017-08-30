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
using Windows.UI.Xaml.Markup;
using System.Xml;

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

                //var tmp = Application.Current.Resources["ColorPickerItemTemplate"];
                var tmp = Windows.UI.Xaml.Markup.XamlReader.Load(
                    @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                        <Grid Background=""{Binding Color16Str}"">
                            <!--<LinearGradientBrush EndPoint=""1, 0.5"" StartPoint=""0, 0.5"">
                                <GradientStop Color=""#FFFFFFFF""/>
                                <GradientStop Color=""{Binding Color16Str}"" Offset=""1""/>
                            </LinearGradientBrush>-->
                            <!--<Grid.Background>
                                <SolidColorBrush Color=""{Binding Color}""/>
                            </Grid.Background>-->
                            <TextBlock Text=""{Binding Name}""/>
                        </Grid>
                    </DataTemplate>");

                ctrl.ItemTemplate = tmp as Windows.UI.Xaml.DataTemplate;
                SetNativeControl(ctrl);
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
