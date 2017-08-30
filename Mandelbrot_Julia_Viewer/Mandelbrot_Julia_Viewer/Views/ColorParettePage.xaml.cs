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
                    colorParette = new Mandelbrot_Julia.ColorResolutionStruct[value.GetLength(0)];
                    value.CopyTo(colorParette, 0);

                    foreach (var parette in ColorParette)
                    {
                        var layout = new StackLayout();
                        layout.Orientation = StackOrientation.Horizontal;
                        layout.BackgroundColor = Color.DarkKhaki;
                        layout.HorizontalOptions = LayoutOptions.FillAndExpand;
                        var slider = new Slider();
                        slider.Value = parette.Posision;
                        slider.HorizontalOptions = LayoutOptions.FillAndExpand;
                        layout.Children.Add(slider);
                        var colsel = new ColorPicker();
                        layout.Children.Add(colsel);
                        StackLayout.Children.Add(layout);
                    }
                }
            }
        }

        public ColorParettePage()
        {
            InitializeComponent();
        }
        public ColorParettePage(MJViewModel viewModel)
        {
            InitializeComponent();

            this.BindingContext = viewModel;
            ColorParette = viewModel.ColorParette;
        }
    }
}