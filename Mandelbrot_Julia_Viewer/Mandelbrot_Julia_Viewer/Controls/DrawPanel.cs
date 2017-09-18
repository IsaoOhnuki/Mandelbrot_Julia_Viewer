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

        public class DrawImageStruct
        {
            public byte[] Image { get; set; }
            public int ImageSizeX { get; set; }
            public int ImageSizeY { get; set; }
        }

        public DrawImageStruct DrawImage { get { return new DrawPanel.DrawImageStruct { Image = ImageData, ImageSizeX = ImageWidth, ImageSizeY = ImageHeight }; } }

        public bool ImageDataValid { get { return ImagePixelOfByteSize > 0 && ImageData?.Length == ImageWidth * ImageHeight * ImagePixelOfByteSize; } }

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
                DrawPanel obj = bindable as DrawPanel;
                if (obj != null && obj.ImageDataValid)
                {
                    obj.OnPropertyChanged(nameof(DrawImage));
                }
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
                DrawPanel obj = bindable as DrawPanel;
                if (obj != null && obj.ImageDataValid)
                {
                    obj.OnPropertyChanged(nameof(DrawImage));
                }
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
                DrawPanel obj = bindable as DrawPanel;
                if (obj != null && obj.ImageDataValid)
                {
                    obj.OnPropertyChanged(nameof(DrawImage));
                }
            });

        public int ImagePixelOfByteSize
        {
            get { return (int)GetValue(ImagePixelOfByteSizeProperty); }
            set { SetValue(ImagePixelOfByteSizeProperty, value); }
        }

        public static readonly BindableProperty ImagePixelOfByteSizeProperty = BindableProperty.Create(
            nameof(ImagePixelOfByteSize),
            typeof(int),
            typeof(DrawPanel),
            4,
            propertyChanged: (bindable, oldValue, newValue) => {
            });
    }
}
