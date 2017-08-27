using Mandelbrot_Julia_Viewer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Mandelbrot_Julia_Viewer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new TopPage());
            //MainPage = new Mandelbrot_Julia_Viewer.Views.StartPage();
            //MainPage = new Mandelbrot_Julia_Viewer.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
