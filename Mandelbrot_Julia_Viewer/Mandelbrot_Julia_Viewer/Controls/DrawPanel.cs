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

        //public Point DrawOrigin
        //{
        //    get { return (Point)GetValue(DrawOriginProperty); }
        //    set { SetValue(DrawOriginProperty, value); }
        //}

        //public static readonly BindableProperty DrawOriginProperty = BindableProperty.Create(
        //    nameof(DrawOrigin),
        //    typeof(Point),
        //    typeof(DrawPanel),
        //    default(Point),
        //    propertyChanged: (bindable, oldValue, newValue) => {
        //    });

        //public enum Scaling
        //{
        //    Infinite,
        //    Limited,
        //}

        //public double DrawScale
        //{
        //    get { return (double)GetValue(DrawScaleProperty); }
        //    set { SetValue(DrawScaleProperty, value); }
        //}

        //public static readonly BindableProperty DrawScaleProperty = BindableProperty.Create(
        //    nameof(DrawScale),
        //    typeof(double),
        //    typeof(DrawPanel),
        //    default(double),
        //    propertyChanged: (bindable, oldValue, newValue) => {
        //    });

        public class DrawImage
        {
            public byte[] Image { get; set; }
            public int ImageSizeX { get; set; }
            public int ImageSizeY { get; set; }
            public Rectangle DrawRect { get; set; }
            public double Scale { get; set; }
            public Point Origin { get; set; }
        }

        public DrawImage GetDrawImage(Point Origin, double Scale, Size Diameter, bool InfiniteScaling)
        {
            var ret = new DrawImage();

            ret.ImageSizeX = (int)(Diameter.Width / Scale);
            ret.ImageSizeY = (int)(Diameter.Height / Scale);

            if (ret.ImageSizeX < ImageWidth && ret.ImageSizeY < ImageHeight)
            {
                ret.DrawRect = new Rectangle(Point.Zero, Diameter);
            }
            else
            {
                ret.DrawRect = new Rectangle((Diameter.Width - (ImageWidth * Scale)) / 2, (Diameter.Height - (ImageHeight * Scale)) / 2, ImageWidth * Scale, ImageHeight * Scale);
            }

            int startX = (int)(ImageWidth / 2 + Origin.X) - ret.ImageSizeX / 2;
            int startY = (int)(ImageHeight / 2 + Origin.Y) - ret.ImageSizeY / 2;

            if (startX < 0)
            {
                Origin.X += -startX;
                startX = 0;
            }
            if (startY < 0)
            {
                Origin.Y += -startY;
                startY = 0;
            }

            ret.Origin = Origin;

            byte[] image = new byte[ret.ImageSizeX * ret.ImageSizeY * 4];

            for (int y = 0; y < ret.ImageSizeY; ++y)
            {
                for (int x = 0; x < ret.ImageSizeX; ++x)
                {
                    image[(y * ret.ImageSizeX + x) * 4]     = ImageData[((startY + y) * ImageWidth + startX + x) * 4];
                    image[(y * ret.ImageSizeX + x) * 4 + 1] = ImageData[((startY + y) * ImageWidth + startX + x) * 4 + 1];
                    image[(y * ret.ImageSizeX + x) * 4 + 2] = ImageData[((startY + y) * ImageWidth + startX + x) * 4 + 2];
                    image[(y * ret.ImageSizeX + x) * 4 + 3] = ImageData[((startY + y) * ImageWidth + startX + x) * 4 + 3];
                }
            }

            ret.Image = image;

            return ret;
        }

        public bool ImageDataValid { get { return ImagePixelOfByteSize > 0 && ImageOrigin.X != double.NaN && ImageOrigin.Y != double.NaN && ImageData?.Length == ImageWidth * ImageHeight * ImagePixelOfByteSize; } }

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

        public Point ImageOrigin
        {
            get { return (Point)GetValue(ImageOriginProperty); }
            set { SetValue(ImageOriginProperty, value); }
        }

        public static readonly BindableProperty ImageOriginProperty = BindableProperty.Create(
            nameof(ImageOrigin),
            typeof(Point),
            typeof(DrawPanel),
            default(Point),
            propertyChanged: (bindable, oldValue, newValue) => {
            });
    }
}
