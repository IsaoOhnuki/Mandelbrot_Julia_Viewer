using System.ComponentModel;
using System.Diagnostics;
using Mandelbrot_Julia_Viewer.UWP.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Behaviors;

[assembly: ResolutionGroupName("Mandelbrot_Julia_Viewer")]
[assembly: ExportEffect(typeof(TappedAndPointEffect), nameof(TappedAndPointEffect))]
namespace Mandelbrot_Julia_Viewer.UWP.Effects
{
    internal class TappedAndPointEffect : PlatformEffect
    {
        private Command<Point> TappedAndPointCommand;
        protected override void OnAttached()
        {
            var c = this.Container as Windows.UI.Xaml.FrameworkElement;
            if (c != null)
            {
                c.Tapped += OnTapped; ;
            }
        }

        protected override void OnDetached()
        {
            var c = this.Container as Windows.UI.Xaml.FrameworkElement;
            if (c != null)
            {
                c.Tapped -= OnTapped; ;
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            TappedAndPointCommand = TappedAndPoint.GetCommand(Element);
        }

        private void OnTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var tap = TappedAndPointCommand;
            if (tap != null)
            {
                var uwpPoint = e.GetPosition((this.Container as Windows.UI.Xaml.FrameworkElement));
                Point point = new Point(uwpPoint.X, uwpPoint.Y);
                Debug.WriteLine(string.Format("Tap detected at position {0}:{1}", point.X, point.Y));
                if (tap.CanExecute(point))
                    tap.Execute(point);
            }
        }

    }
}