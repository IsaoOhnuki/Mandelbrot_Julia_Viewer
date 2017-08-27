using Mandelbrot_Julia_Viewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mandelbrot_Julia_Viewer.Views
{
    public partial class MainPage : ContentPage
    {
 
        private MJViewModel viewModel;
        public MJViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                viewModel = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            Title = "MainPage";

            ViewModel = new MJViewModel();
            InitializeComponent();

            BindingContext = ViewModel;
        }
    }
}
