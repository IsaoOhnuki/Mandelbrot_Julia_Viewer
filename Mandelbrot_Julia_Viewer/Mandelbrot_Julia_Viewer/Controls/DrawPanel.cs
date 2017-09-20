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

        private Size viewSize;
        public Size ViewSize
        {
            get { return viewSize; }
            set
            {
                viewSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceImage));
            }
        }

        private Point viewOrigin;
        public Point ViewOrigin
        {
            get { return viewOrigin; }
            set
            {
                viewOrigin = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceImage));
            }
        }

        private double viewScale = 1;
        public double ViewScale
        {
            get { return viewScale; }
            set
            {
                double oldScale = ViewScale;
                viewScale = value;

                //if (scale.Scale < 0.001)
                //    scale.Scale = 0.001;

                //double width = DrawImage.ImageSizeX * Scale.Scale;
                //double height = DrawImage.ImageSizeY * Scale.Scale;

                //double distanceX = Scale.Position.X * Scale.Scale;
                //double distanceY = Scale.Position.Y * Scale.Scale;

                ////Origin = new Point(Origin.X + (drawRect.Width - width) / 2 + distanceX, Origin.Y + (drawRect.Height - height) / 2 + distanceY);
                //Origin = new Point(Origin.X + (drawRect.Width - width) / 2, Origin.Y + (drawRect.Height - height) / 2);

                //drawRect.Width = width;
                //drawRect.Height = height;

                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceImage));
            }
        }

        public Rectangle ViewRect
        {
            get
            {
                return new Rectangle
                {
                    X = 
                }
            }
        }

        public class DeviceImageStruct
        {
            public object Image { get; set; }
            public Rectangle ViewRect { get; set; }
            public Rectangle DrawRect { get; set; }
        }

        public DeviceImageStruct DeviceImage { get; set; }
        public Func<byte[], int, int, int, Task<object>> ImageCompaile;

        //public class ImageStruct
        //{
        //    public byte[] Image { get; set; }
        //    public int ImageSizeX { get; set; }
        //    public int ImageSizeY { get; set; }
        //}
        //public ImageStruct DrawImage { get { return new DrawPanel.ImageStruct { Image = ImageData, ImageSizeX = ImageWidth, ImageSizeY = ImageHeight }; } }

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
            propertyChanged: async(bindable, oldValue, newValue) => {
                DrawPanel obj = bindable as DrawPanel;
                if (obj != null && obj.ImageDataValid)
                {
                    obj.DeviceImage = new DeviceImageStruct { Image = await obj.ImageCompaile?.Invoke(obj.ImageData, obj.ImageWidth, obj.ImageHeight, obj.ImagePixelOfByteSize) };
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
                    obj.DeviceImage = new DeviceImageStruct { Image = obj.ImageCompaile?.Invoke(obj.ImageData, obj.ImageWidth, obj.ImageHeight, obj.ImagePixelOfByteSize) };
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
                    obj.DeviceImage = new DeviceImageStruct { Image = obj.ImageCompaile?.Invoke(obj.ImageData, obj.ImageWidth, obj.ImageHeight, obj.ImagePixelOfByteSize) };
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
                DrawPanel obj = bindable as DrawPanel;
                if (obj != null && obj.ImageDataValid)
                {
                    obj.DeviceImage = new DeviceImageStruct { Image = obj.ImageCompaile?.Invoke(obj.ImageData, obj.ImageWidth, obj.ImageHeight, obj.ImagePixelOfByteSize) };
                }
            });
    }
}
