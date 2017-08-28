using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mandelbrot_Julia_Viewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorSelectPage : ContentPage
    {
        public ColorSelectPage()
        {
            InitializeComponent();
        }
    }
}