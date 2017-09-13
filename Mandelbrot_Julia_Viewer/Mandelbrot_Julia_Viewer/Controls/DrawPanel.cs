using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Controls
{
    public class DrawPanel : View
    {
        // http://qiita.com/AyaseSH/items/52768d4a9f22f417642f Xamarin.FormsでPinchGestureRecognizerのユーザー操作について

        public byte[] ImageData
        {
            get { return (byte[])GetValue(ImageDataProperty); }
            set { SetValue(ImageDataProperty, value); }
        }

        public static readonly BindableProperty ImageDataProperty = BindableProperty.Create(
            nameof(ImageData),
            typeof(byte[]),
            typeof(DrawPanel),
            default(byte[]),
            propertyChanged: (bindable, oldValue, newValue) => {
            });

        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly BindableProperty ImageWidthProperty = BindableProperty.Create(
            nameof(ImageWidth),
            typeof(int),
            typeof(DrawPanel),
            default(int),
            propertyChanged: (bindable, oldValue, newValue) => {
            });

        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly BindableProperty ImageHeightProperty = BindableProperty.Create(
            nameof(ImageHeight),
            typeof(int),
            typeof(DrawPanel),
            default(int),
            propertyChanged: (bindable, oldValue, newValue) => {
            });
    }
}
