using Controls;
using Mandelbrot_Julia_Viewer.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mandelbrot_Julia_Viewer.Views
{
    // http://itblogdsi.blog.fc2.com/blog-entry-194.html PickerにHorizontalTextAlignmentを実装する方法 | Xamarin.Forms

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorParettePage : ContentPage
    {
        private Mandelbrot_Julia.ColorResolutionStruct[] colorParette;
        public Mandelbrot_Julia.ColorResolutionStruct[] ColorParette
        {
            get { return colorParette; }
            set
            {
                colorParette = null;
                if (value != null)
                {
                    colorParette = value;
                    //colorParette = new Mandelbrot_Julia.ColorResolutionStruct[value.GetLength(0)];
                    //value.CopyTo(colorParette, 0);

                    foreach (var parette in ColorParette)
                    {
                        var layout = new Grid();
                        layout.BindingContext = parette;
                        layout.ColumnDefinitions = new ColumnDefinitionCollection();
                        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
                        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        layout.BackgroundColor = Color.DarkKhaki;
                        layout.HorizontalOptions = LayoutOptions.FillAndExpand;
                        var slider = new Slider();
                        slider.SetBinding(Slider.ValueProperty, new Binding("Posision", BindingMode.TwoWay));
                        slider.HorizontalOptions = LayoutOptions.FillAndExpand;
                        slider.SetValue(Grid.ColumnProperty, 0);
                        layout.Children.Add(slider);
                        var colsel = new ColorPicker();
                        colsel.SetBinding(ColorPicker.SelectedColorProperty, new Binding("Color", BindingMode.TwoWay));
                        colsel.SelectedColor = parette.Color;
                        colsel.SetValue(Grid.ColumnProperty, 1);
                        layout.Children.Add(colsel);
                        StackLayout.Children.Add(layout);
                    }
                }
            }
        }

        public ColorParettePage()
        {
            InitializeComponent();
            ToolbarItems.Add(new ToolbarItem("<", "", () => { }, ToolbarItemOrder.Default));
        }
        public ColorParettePage(MJViewModel viewModel)
        {
            InitializeComponent();

            ColorParette = viewModel.ColorParette;
            ToolbarItems.Add(new ToolbarItem("←", "", () => { ((MasterDetailPage)Parent).Detail = new DrawPage(viewModel); }, ToolbarItemOrder.Default));
        }
    }
}