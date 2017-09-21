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
        public struct Matrix2
        {
            double m11;
            double m12;
            double m13;
            double m21;
            double m22;
            double m23;
            double dx;
            double dy;
            double dd;
            public static Matrix2 operator *(Matrix2 l, Matrix2 r)
            {
                return new Matrix2();
            }
            public Matrix2(double m11, double m12, double m21, double m22, double dx, double dy)
            {
                this.m11 = m11;
                this.m12 = m12;
                this.m13 = 0;
                this.m21 = m21;
                this.m22 = m22;
                this.m23 = 0;
                this.dx = dx;
                this.dy = dy;
                this.dd = 1;
            }
            public static Size Calc(Size point, Matrix2 matrix)
            {
                return new Size { Width = point.Width * matrix.m11 + point.Width * matrix.m12 + matrix.dx, Height = point.Height * matrix.m21 + point.Height * matrix.m22 + matrix.dy };
            }
            public static Point Calc(Point point, Matrix2 matrix)
            {
                return new Point { X = point.X * matrix.m11 + point.X * matrix.m12 + matrix.dx, Y = point.Y * matrix.m21 + point.Y * matrix.m22 + matrix.dy };
            }
            public static Point Enlargement(Point point, double x, double y)
            {
                return Matrix2.Calc(point, new Matrix2 { m11 = x, m12 = 0, m21 = 0, m22 = y, dx = 0, dy = 0 });
            }
            public static Point Move(Point point, double x, double y)
            {
                return Matrix2.Calc(point, new Matrix2 { m11 = 1, m12 = 0, m21 = 0, m22 = 1, dx = x, dy = y });
            }
            public static Point Rotate(Point point, double o)
            {
                return Matrix2.Calc(point, new Matrix2 { m11 = Math.Cos(o), m12 = -Math.Sin(o), m21 = Math.Sin(o), m22 = Math.Cos(o), dx = 0, dy = 0 });
            }
            public static Size Enlargement(Size size, double x, double y)
            {
                return Matrix2.Calc(size, new Matrix2 { m11 = x, m12 = 0, m21 = 0, m22 = y, dx = 0, dy = 0 });
            }
            public static Size Move(Size size, double x, double y)
            {
                return Matrix2.Calc(size, new Matrix2 { m11 = 1, m12 = 0, m21 = 0, m22 = 1, dx = x, dy = y });
            }
            public static Size Rotate(Size size, double o)
            {
                return Matrix2.Calc(size, new Matrix2 { m11 = Math.Cos(o), m12 = -Math.Sin(o), m21 = Math.Sin(o), m22 = Math.Cos(o), dx = 0, dy = 0 });
            }
        }

        // http://qiita.com/AyaseSH/items/52768d4a9f22f417642f Xamarin.FormsでPinchGestureRecognizerのユーザー操作について

        private Rectangle viewRect;
        public Rectangle ViewRect
        {
            get { return viewRect; }
            set
            {
                viewRect = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceImage));
            }
        }

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
                viewScale = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceImage));
            }
        }

        public DeviceImageStruct GetDeviceImage()
        {
            return new DeviceImageStruct
            {

            };
        }

        public class DeviceImageStruct
        {
            public object Image { get; set; }
            public Rectangle ViewRect { get; set; }
            public Rectangle DrawRect { get; set; }
        }

        public Func<byte[], int, int, int, Task<object>> ImageCompaile;
        public object DeviceImage { get; set; }

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
