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
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new ComboBox();

                //var tmp = Application.Current.Resources["ColorPickerItemTemplate"];
                var tmp = Windows.UI.Xaml.Markup.XamlReader.Load(
                    @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                        <!--<Grid Background=""{Binding Color16Str}"">
                            <TextBlock Text=""{Binding Name}""/>
                        </Grid>-->
                        <Grid CornerRadius=""5"">
                            <Grid.Background>
                                <LinearGradientBrush EndPoint=""0.6, 0.5"" StartPoint=""0, 0.5"">
                                    <GradientStop Color=""#FFFFFFFF""/>
                                    <GradientStop Color=""{Binding Color16Str}"" Offset=""1""/>
                                </LinearGradientBrush>
                            </Grid.Background>
                            <TextBlock Text=""{Binding Name}"" Padding=""5""/>
                        </Grid>
                    </DataTemplate>");

                ctrl.ItemTemplate = tmp as Windows.UI.Xaml.DataTemplate;
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                Control.SelectionChanged -= Control_SelectionChanged;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.ItemsSource = e.NewElement.ListItems;
                Control.SelectedItem = (Control.ItemsSource as IEnumerable<ColorStruct>)?.Where(x => x.Color == e.NewElement.SelectedColor).SingleOrDefault();
                Control.SelectionChanged += Control_SelectionChanged;
            }
            base.OnElementChanged(e);
        }

        private void Control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.AddedItems[0] is ColorStruct)
                Element.SelectedColor = (e.AddedItems[0] as ColorStruct).Color;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // プロパティ値の変更を反映
            if (e.PropertyName == ColorPicker.SelectedColorProperty.PropertyName)
            {
                Control.SelectedItem = (Control.ItemsSource as IEnumerable<ColorStruct>)?.Where(x => x.Color == Element.SelectedColor).SingleOrDefault();
            }
        }
    }
}
