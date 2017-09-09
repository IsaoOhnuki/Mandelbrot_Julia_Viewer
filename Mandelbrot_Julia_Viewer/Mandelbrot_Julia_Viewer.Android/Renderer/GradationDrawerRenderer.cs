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
using Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System.ComponentModel;
using Android.Graphics.Drawables;
using System.Collections.Specialized;

[assembly: ExportRenderer(typeof(GradationDrawer), typeof(Mandelbrot_Julia_Viewer.Droid.GradationDrawerRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    // http://qiita.com/croquette0212/items/24dc2b6de3730e831aab AndroidのSurfaceViewの基礎
    // http://furuya02.hatenablog.com/entry/2014/11/23/001448 Xamarin.Forms 描画で考慮すべき２つのこと


    public class GradationDrawerRenderer : ViewRenderer<GradationDrawer, Android.Views.View>
    //public class GradationDrawerRenderer : ViewRenderer<GradationDrawer, Android.Views.SurfaceView>, ISurfaceHolderCallback
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GradationDrawer> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new Android.Views.View(this.Context);
                SetWillNotDraw(false); // Android.Views.View継承で必ず指定する
                //var ctrl = new Android.Views.SurfaceView(this.Context);
                //ctrl.Holder.AddCallback(this);
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                if (e.OldElement.Colors is IList<GradationDrawer.ColPos>)
                {
                    foreach (var val in e.OldElement.Colors as IList<GradationDrawer.ColPos>)
                    {
                        val.PropertyChanged -= Colors_PropertyChanged;
                    }
                }
                if (e.OldElement.Colors is INotifyCollectionChanged)
                    (e.OldElement.Colors as INotifyCollectionChanged).CollectionChanged -= Colors_CollectionChanged;
            }
            if (Control != null && e.NewElement != null)
            {
                if (e.NewElement.Colors is IList<GradationDrawer.ColPos>)
                {
                    foreach (var val in e.NewElement.Colors as IList<GradationDrawer.ColPos>)
                    {
                        val.PropertyChanged += Colors_PropertyChanged;
                    }
                }
                if (e.NewElement.Colors is INotifyCollectionChanged)
                    (e.NewElement.Colors as INotifyCollectionChanged).CollectionChanged += Colors_CollectionChanged;
            }
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Invalidate();
        }

        private void Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invalidate();
        }

        private void Colors_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GradationDrawer.ColPos.Color))
            {

            }
            if (e.PropertyName == nameof(GradationDrawer.ColPos.Position))
            {

            }
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var cols = Element.CreateColorArray((int)Height).Select(x => new Android.Graphics.Color { A = 255, R = (byte)(x.R * 255.0), G = (byte)(x.G * 255.0), B = (byte)(x.B * 255.0) }).ToArray();
            var paint = new Paint();

            paint.Color = Android.Graphics.Color.Honeydew;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new Rect(0, 0, Width, Height), paint);
            for (int y = 0; y < (int)Height; ++y)
            {
                paint.Color = cols[y];
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeWidth = 1;
                canvas.DrawLine(0, y, (int)Width / 2, y, paint);
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            Paint paint = new Paint(PaintFlags.AntiAlias);
            paint.Color = Android.Graphics.Color.Blue;
            paint.SetStyle(Paint.Style.Fill);

            Canvas canvas = holder.LockCanvas();
            canvas.DrawColor(Android.Graphics.Color.Black);
            canvas.DrawCircle(100, 200, 50, paint);
            holder.UnlockCanvasAndPost(canvas);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            Paint paint = new Paint(PaintFlags.AntiAlias);
            paint.Color = Android.Graphics.Color.Blue;
            paint.SetStyle(Paint.Style.Fill);

            Canvas canvas = holder.LockCanvas();
            canvas.DrawColor(Android.Graphics.Color.Black);
            canvas.DrawCircle(100, 200, 50, paint);
            holder.UnlockCanvasAndPost(canvas);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
        }
    }
}