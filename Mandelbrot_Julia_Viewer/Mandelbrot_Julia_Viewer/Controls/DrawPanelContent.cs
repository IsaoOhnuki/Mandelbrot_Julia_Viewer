using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Controls
{
    public interface IDrawPanelContent
    {

    }

    public interface IDeviceBitmapCreator
    {
        Task<object> Create(byte[] imageData, int imageWidth, int imageHeight, int pixelByteCount);
    }

    public class SimpleDrawPanelContent : Element, IDrawPanelContent
    {
        public class DrawImageStruct
        {
            public object Image { get; set; }
            public Rectangle DrawRect { get; set; }
        }

        public Task<DrawImageStruct> DrawImmageRequestAsync(Point reqPoint, Size reqSize)
        {
            return Task.Run(() => {
                Rectangle imgRect = new Rectangle(reqPoint, reqSize);
                Rectangle drawRect = Rectangle.Intersect(imgRect, ImageRect);

                return new DrawImageStruct
                {
                    Image = DeviceImage,
                    DrawRect = drawRect
                };
            });
        }

        public DrawImageStruct DrawImmageRequest(Point reqPoint, Size reqSize)
        {
            Rectangle imgRect = new Rectangle(reqPoint, reqSize);
            Rectangle drawRect = Rectangle.Intersect(imgRect, ImageRect);

            return new DrawImageStruct
            {
                Image = DeviceImage,
                DrawRect = drawRect
            };
        }

        private object deviceImage;
        public object DeviceImage
        {
            get { return deviceImage; }
            set
            {
                if (deviceImage is IDisposable)
                {
                    (deviceImage as IDisposable).Dispose();
                }
                deviceImage = value;
                OnPropertyChanged();
            }
        }
        public async void DeviceImageCompile()
        {
            var bitmapCreator = DependencyService.Get<IDeviceBitmapCreator>();
            DeviceImage = await bitmapCreator.Create(ImageData, ImageWidth, ImageHeight, ImagePixelOfByteSize);
        }

        public bool ImageDataValid { get { return ImagePixelOfByteSize > 0 && ImageData?.Length == ImageWidth * ImageHeight * ImagePixelOfByteSize; } }

        public Rectangle ImageRect { get { return new Rectangle(0, 0, ImageWidth, ImageHeight); } }

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
                if (bindable is DrawPanel obj && obj.ImageDataValid)
                {
                    obj.DeviceImageCompile();
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
                if (bindable is DrawPanel obj && obj.ImageDataValid)
                {
                    obj.DeviceImageCompile();
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
                if (bindable is DrawPanel obj && obj.ImageDataValid)
                {
                    obj.DeviceImageCompile();
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
            defaultValue: 4,
            propertyChanged: (bindable, oldValue, newValue) => {
                if (bindable is DrawPanel obj && obj.ImageDataValid)
                {
                    obj.DeviceImageCompile();
                }
            });
    }
}
