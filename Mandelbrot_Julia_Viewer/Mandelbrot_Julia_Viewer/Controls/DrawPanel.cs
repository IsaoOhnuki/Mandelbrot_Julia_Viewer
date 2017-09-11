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
        public event EventHandler<ImageChangedEventArgs> ImageChanged;

        // BindablePropertyを追加
        public object Image
        {
            get { return GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly BindableProperty ImageProperty = BindableProperty.Create(
            nameof(Image),
            typeof(object),
            typeof(DrawPanel),
            default(object),
            propertyChanged: (bindable, oldValue, newValue) => {
                ((DrawPanel)bindable).ImageChanged?.Invoke(bindable, new ImageChangedEventArgs((object)newValue));
            });
    }

    public class ImageChangedEventArgs : EventArgs
    {
        public ImageChangedEventArgs(object newValue)
        {
            NewValue = newValue;
        }
        public object NewValue { get; private set; }
    }
}
