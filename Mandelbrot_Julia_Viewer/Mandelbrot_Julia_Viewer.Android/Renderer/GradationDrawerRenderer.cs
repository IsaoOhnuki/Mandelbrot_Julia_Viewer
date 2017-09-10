//#define UseSurfaceView

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
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(GradationDrawer), typeof(Mandelbrot_Julia_Viewer.Droid.GradationDrawerRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    // http://qiita.com/croquette0212/items/24dc2b6de3730e831aab AndroidのSurfaceViewの基礎
    // http://furuya02.hatenablog.com/entry/2014/11/23/001448 Xamarin.Forms 描画で考慮すべき２つのこと

#if UseSurfaceView
    public class GradationDrawerRenderer : ViewRenderer<GradationDrawer, Android.Views.SurfaceView>, ISurfaceHolderCallback
#else
    public class GradationDrawerRenderer : ViewRenderer<GradationDrawer, Android.Views.View>
#endif
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GradationDrawer> e)
        {
            if (Control == null && e.NewElement != null)
            {
#if UseSurfaceView
                var ctrl = new Android.Views.SurfaceView(this.Context);
                ctrl.Holder.AddCallback(this);
#else
                var ctrl = new Android.Views.View(this.Context);
                SetWillNotDraw(false); // Android.Views.View継承で必ず指定する
#endif
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                if (e.OldElement.Colors is IList<GradationDrawer.ColPos>)
                {
                    foreach (var val in e.OldElement.Colors as IList<GradationDrawer.ColPos>)
                    {
                        val.PropertyChanged -= Colors_PropertyChangedAsync;
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
                        val.PropertyChanged += Colors_PropertyChangedAsync;
                    }
                }
                if (e.NewElement.Colors is INotifyCollectionChanged)
                    (e.NewElement.Colors as INotifyCollectionChanged).CollectionChanged += Colors_CollectionChanged;
            }
            base.OnElementChanged(e);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == GradationDrawer.HeightProperty.PropertyName)
            {
                Gradation = await GetGradation();
                Invalidate();
            }
            if (e.PropertyName == GradationDrawer.WidthProperty.PropertyName)
            {
                Gradation = await GetGradation();
                Invalidate();
            }
        }

        private async void Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var parette in e.NewItems)
                {
                    ((GradationDrawer.ColPos)parette).PropertyChanged += Colors_PropertyChangedAsync;
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var parette in e.OldItems)
                {
                    ((GradationDrawer.ColPos)parette).PropertyChanged -= Colors_PropertyChangedAsync;
                }
            }
            Gradation = await GetGradation();
            Invalidate();
        }

        private async void Colors_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GradationDrawer.ColPos.Color))
            {
                Gradation = await GetGradation();
                Invalidate();
            }
            if (e.PropertyName == nameof(GradationDrawer.ColPos.Position))
            {
                Gradation = await GetGradation();
                Invalidate();
            }
        }

        private void DrawGradation(Canvas canvas)
        {
            Paint paint = new Paint(PaintFlags.AntiAlias);

            paint.Color = Android.Graphics.Color.Honeydew;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new Rect(0, 0, Width, Height), paint);
            for (int y = 0; y < Gradation?.Length; ++y)
            {
                paint.Color = Gradation[y];
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeWidth = 1;
                canvas.DrawLine(0, y, (int)Element.Width / 2, y, paint);
            }
        }

        public Android.Graphics.Color[] Gradation { get; set; }

        private Task<Android.Graphics.Color[]> GetGradation()
        {
            return Task<Android.Graphics.Color[]>.Run(() => {
                return Element.CreateColorArray((int)Element.Height).Select(x => new Android.Graphics.Color { A = 255, R = (byte)(x.R * 255.0), G = (byte)(x.G * 255.0), B = (byte)(x.B * 255.0) }).ToArray();
            });
        }

#if UseSurfaceView
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            Canvas canvas = holder.LockCanvas();
            DrawGradation(canvas);
            holder.UnlockCanvasAndPost(canvas);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            //Canvas canvas = holder.LockCanvas();
            //DrawGradation(canvas);
            //holder.UnlockCanvasAndPost(canvas);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
        }
#else
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            DrawGradation(canvas);
        }
#endif
    }
}