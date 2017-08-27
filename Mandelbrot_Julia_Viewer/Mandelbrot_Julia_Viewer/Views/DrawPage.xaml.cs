using Mandelbrot_Julia_Viewer.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mandelbrot_Julia_Viewer.Views
{
    // http://kamusoft.hatenablog.jp/entry/2016/12/31/014051 Xamarin.FormsのPan/PinchGestureRecognizerを使ってドラッグアンドドロップと拡大縮小を実装するサンプル
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawPage : ContentPage
    {
        public DrawPage()
        {
            InitializeComponent();
        }
        public DrawPage(MJViewModel viewModel)
        {
            InitializeComponent();

            this.BindingContext = viewModel;
        }
    }
}