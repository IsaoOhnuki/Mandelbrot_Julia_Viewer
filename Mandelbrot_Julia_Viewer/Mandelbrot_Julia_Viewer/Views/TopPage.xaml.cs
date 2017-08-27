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
    public partial class TopPage : MasterDetailPage
    {
        // https://code.msdn.microsoft.com/windowsapps/Xamarin02-7b511b90 Xamarin入門#02 HamburgerMenu

        public TopPage()
        {
            InitializeComponent();

            Master = new MainPage();
            Detail = new DrawPage(((MainPage)Master).ViewModel);
        }
    }
}