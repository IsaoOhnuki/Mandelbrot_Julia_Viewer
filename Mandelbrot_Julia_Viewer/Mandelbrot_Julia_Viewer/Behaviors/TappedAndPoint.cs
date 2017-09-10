using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Behaviors
{
    // https://forums.xamarin.com/discussion/17767/touch-coordinates-in-tapgesturerecognizer
    // xaml xmlns:Behaviors="clr-namespace:Behaviors;assembly=ソリューション"
    //      <CONTROL Behaviors:TappedAndPoint.Tapped="{Binding CanvasTappedCommand}" />
    // code public Command<Point> CanvasTappedCommand { get { return new Command<Point>((p) => OnCanvasTapped(p)); } }
    //      public void OnCanvasTapped(Point p)
    //      {
    //          // your event handling logic
    //      }

    public static class TappedAndPoint
    {
        public static readonly BindableProperty TappedProperty = BindableProperty.CreateAttached("Tapped", typeof(Command<Point>), typeof(TappedAndPoint), null, propertyChanged: CommandChanged);

        public static Command<Point> GetCommand(BindableObject view)
        {
            return (Command<Point>)view.GetValue(TappedProperty);
        }

        public static void SetTapped(BindableObject view, Command<Point> value)
        {
            view.SetValue(TappedProperty, value);
        }

        private static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view != null)
            {
                var effect = GetOrCreateEffect(view);
            }
        }

        private static TappedAndPointEffect GetOrCreateEffect(View view)
        {
            var effect = (TappedAndPointEffect)view.Effects.FirstOrDefault(e => e is TappedAndPointEffect);
            if (effect == null)
            {
                effect = new TappedAndPointEffect();
                view.Effects.Add(effect);
            }
            return effect;
        }

        class TappedAndPointEffect : RoutingEffect
        {
            public TappedAndPointEffect() : base("Mandelbrot_Julia_Viewer.TappedAndPointEffect")
            {
            }
        }
    }
}
