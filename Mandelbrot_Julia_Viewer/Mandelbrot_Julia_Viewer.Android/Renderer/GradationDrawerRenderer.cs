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

[assembly: ExportRenderer(typeof(GradationDrawer), typeof(Mandelbrot_Julia_Viewer.Droid.GradationDrawerRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    // http://qiita.com/croquette0212/items/24dc2b6de3730e831aab AndroidのSurfaceViewの基礎

    public class GradationDrawerRenderer : ViewRenderer<GradationDrawer, SurfaceView>, ISurfaceHolderCallback
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GradationDrawer> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new Android.Views.SurfaceView(this.Context);
                ctrl.Holder.AddCallback(this);
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
            }
            if (Control != null && e.NewElement != null)
            {
            }
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
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

    //public class ColorPickerArrayAdapter : ArrayAdapter<ColorStruct>
    //{
    //    public ColorPickerArrayAdapter(Context context, int textViewResourceId, IList<ColorStruct> objects)
    //        : base(context, textViewResourceId, objects)
    //    {

    //    }
    //    public override Android.Views.View GetDropDownView(int position, Android.Views.View convertView, ViewGroup parent)
    //    {
    //        return getCustomView(position, convertView, parent);
    //    }
    //    public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
    //    {
    //        return getCustomView(position, convertView, parent);
    //    }
    //    public Android.Views.View getCustomView(int position, Android.Views.View convertView, ViewGroup parent)
    //    {
    //        var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
    //        Android.Views.View item = inflater.Inflate(Resource.Layout.ColorPickerItemlayout, parent, false);
    //        TextView label = item.FindViewById<TextView>(Resource.Id.ColorPickerItemText);
    //        label.Text = GetItem(position).Name;
    //        label.SetTextColor(new Android.Graphics.Color(0, 0, 0));
    //        label.SetBackgroundColor(new Android.Graphics.Color(GetItem(position).ColorInt));
    //        return item;
    //    }
    //}
}