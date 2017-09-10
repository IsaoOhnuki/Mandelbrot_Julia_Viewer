using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Android.Support.V4.View;
using Android.Util;
using Xamarin.Forms.Platform.Android;
using Mandelbrot_Julia_Viewer.Droid.Effects;
using System.ComponentModel;
using Behaviors;

[assembly: ResolutionGroupName("Mandelbrot_Julia_Viewer")]
[assembly: ExportEffect(typeof(TappedAndPointEffect), nameof(TappedAndPointEffect))]
namespace Mandelbrot_Julia_Viewer.Droid.Effects
{
    public class TappedAndPointEffect : PlatformEffect
    {
        private GestureDetectorCompat gestureRecognizer;
        private readonly InternalGestureDetector tapDetector;
        private Command<Point> TappedAndPointCommand;
        private DisplayMetrics displayMetrics;

        public TappedAndPointEffect()
        {
            tapDetector = new InternalGestureDetector
            {
                TapAction = motionEvent =>
                {
                    var tap = TappedAndPointCommand;
                    if (tap != null)
                    {
                        var x = motionEvent.GetX();
                        var y = motionEvent.GetY();

                        var point = PxToDp(new Point(x, y));
                        Log.WriteLine(LogPriority.Debug, "gesture", $"Tap detected at {x} x {y} in forms: {point.X} x {point.Y}");
                        if (tap.CanExecute(point))
                            tap.Execute(point);
                    }
                }
            };
        }

        private Point PxToDp(Point point)
        {
            point.X = point.X / displayMetrics.Density;
            point.Y = point.Y / displayMetrics.Density;
            return point;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            TappedAndPointCommand = TappedAndPoint.GetCommand(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            var context = control.Context;
            displayMetrics = context.Resources.DisplayMetrics;
            tapDetector.Density = displayMetrics.Density;

            if (gestureRecognizer == null)
                gestureRecognizer = new GestureDetectorCompat(context, tapDetector);
            control.Touch += ControlOnTouch;

            OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        private void ControlOnTouch(object sender, Android.Views.View.TouchEventArgs touchEventArgs)
        {
            gestureRecognizer?.OnTouchEvent(touchEventArgs.Event);
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.Touch -= ControlOnTouch;
        }

        sealed class InternalGestureDetector : GestureDetector.SimpleOnGestureListener
        {
            public Action<MotionEvent> TapAction { get; set; }
            public float Density { get; set; }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                TapAction?.Invoke(e);
                return true;
            }
        }
    }
}