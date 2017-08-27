using Mandelbrot_Julia_Viewer.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mandelbrot_Julia_Viewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage : ContentPage
    {
        public StartPage()
        {
            InitializeComponent();

            this.BindingContext = new MJViewModel();

            var tapped = new TapGestureRecognizer();
            tapped.Tapped += Tapped_Tapped;
            ImageView.GestureRecognizers.Add(tapped);
        }

        private void Tapped_Tapped(object sender, EventArgs e)
        {
            DisplayAlert("", "Tap", "OK");
        }
    }
}