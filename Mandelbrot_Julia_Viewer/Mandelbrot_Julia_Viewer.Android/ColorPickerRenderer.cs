using System;
using System.Collections.Generic;
using System.ComponentModel;
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

[assembly: ExportRenderer(typeof(ColorPicker), typeof(Mandelbrot_Julia_Viewer.Droid.ColorPickerRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    // http://santea.hateblo.jp/entry/2016/08/19/163020 Custom Renderer で CardView を作ってみた
    // http://qiita.com/Helmos/items/6b65d40d355379dc7ffa Key, Valueの値を持ったSpinnerを作る(XMLから読み込む)

    class ColorPickerRenderer : ViewRenderer<ColorPicker, Spinner>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ColorPicker> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new Spinner(this.Context);
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                Control.ItemSelected -= Control_ItemSelected;
            }
            if (Control != null && e.NewElement != null)
            {
                var adpt = new ColorPickerArrayAdapter(this.Context, Android.Resource.Layout.SimpleSpinnerItem, e.NewElement.ListItems);
                adpt.SetDropDownViewResource(Resource.Id.ColorPickerItem);
                Control.Adapter = adpt;

                Control.SetSelection(e.NewElement.ListItems.IndexOf((e.NewElement.ListItems as IEnumerable<ColorStruct>)?.Where(x => x.Color == e.NewElement.SelectedColor).SingleOrDefault()));
                Control.ItemSelected += Control_ItemSelected;
            }
            base.OnElementChanged(e);
        }

        private void Control_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Element.SelectedColor = Element.ListItems[e.Position].Color;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }

    public class ColorPickerArrayAdapter : ArrayAdapter<ColorStruct>
    {
        public ColorPickerArrayAdapter(Context context, int textViewResourceId, IList<ColorStruct> objects)
            : base(context, textViewResourceId, objects)
        {

        }
        public override Android.Views.View GetDropDownView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            return getCustomView(position, convertView, parent);
        }
        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            return getCustomView(position, convertView, parent);
        }
        public Android.Views.View getCustomView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            Android.Views.View item = inflater.Inflate(Resource.Layout.ColorPickerItemlayout, parent, false);
            TextView label = item.FindViewById<TextView>(Resource.Id.ColorPickerItemText);
            label.Text = GetItem(position).Name;
            label.SetTextColor(new Android.Graphics.Color(0, 0, 0));
            label.SetBackgroundColor(new Android.Graphics.Color(GetItem(position).ColorInt));
            return item;
        }
    }
}