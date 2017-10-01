using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Controls
{
    public class DrawPanel : View
    {
        // http://qiita.com/AyaseSH/items/52768d4a9f22f417642f Xamarin.FormsでPinchGestureRecognizerのユーザー操作について

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
                    Image = deviceImage,
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
                Image = deviceImage,
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

        public Point ViewPoint
        {
            get { return (Point)GetValue(ViewPointProperty); }
            set { SetValue(ViewPointProperty, value); }
        }

        public static readonly BindableProperty ViewPointProperty = BindableProperty.Create(
            nameof(ViewPoint),
            typeof(Point),
            typeof(DrawPanel),
            defaultValue: default(Point),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) => {
            });

        private double minViewScale = 0.01;
        public double MinViewScale
        {
            get { return minViewScale; }
            set
            {
                minViewScale = value;
                OnPropertyChanged();
            }
        }

        private double maxViewScale = double.NaN;
        public double MaxViewScale
        {
            get { return maxViewScale; }
            set
            {
                maxViewScale = value;
                OnPropertyChanged();
            }
        }

        public double ViewScale
        {
            get { return (double)GetValue(ViewScaleProperty); }
            set
            {
                try
                {
                    SetValue(ViewScaleProperty, value);
                }
                catch
                {
                    if (value < MinViewScale)
                        SetValue(ViewScaleProperty, MinViewScale);
                    else if (value > MaxViewScale)
                        SetValue(ViewScaleProperty, MaxViewScale);
                }
            }
        }

        public static readonly BindableProperty ViewScaleProperty = BindableProperty.Create(
            nameof(ViewScale),
            typeof(double),
            typeof(DrawPanel),
            defaultValue: 1d,
            defaultBindingMode: BindingMode.TwoWay,
            validateValue: (bindable, v) => {
                DrawPanel obj = bindable as DrawPanel;
                double value = (double)v;
                return (double.IsNaN(obj.MinViewScale) || value >= obj.MinViewScale) && (double.IsNaN(obj.MaxViewScale) || value <= obj.MaxViewScale);
            },
            propertyChanged: (bindable, oldValue, newValue) => {
            });

        public ICommand TappedCommand
        {
            get { return (ICommand)GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }

        public static readonly BindableProperty TappedCommandProperty = BindableProperty.Create(
            nameof(TappedCommand),
            typeof(ICommand),
            typeof(DrawPanel),
            default(ICommand),
            propertyChanged: (bindable, oldValue, newValue) => {
            });

        public void OnTapped(double x, double y)
        {
            if (TappedCommand != null && TappedCommand.CanExecute(new Point(x, y)))
                TappedCommand?.Execute(new Point(x, y));
        }

        public ICommand DoubleTappedCommand
        {
            get { return (ICommand)GetValue(DoubleTappedCommandProperty); }
            set { SetValue(DoubleTappedCommandProperty, value); }
        }

        public static readonly BindableProperty DoubleTappedCommandProperty = BindableProperty.Create(
            nameof(DoubleTappedCommand),
            typeof(ICommand),
            typeof(DrawPanel),
            default(ICommand),
            propertyChanged: (bindable, oldValue, newValue) => {
            });

        public void OnDoubleTapped(double x, double y)
        {
            if (DoubleTappedCommand != null && DoubleTappedCommand.CanExecute(new Point(x, y)))
                DoubleTappedCommand?.Execute(new Point(x, y));
        }

        public ICommand LongTappedCommand
        {
            get { return (ICommand)GetValue(LongTappedCommandProperty); }
            set { SetValue(LongTappedCommandProperty, value); }
        }

        public static readonly BindableProperty LongTappedCommandProperty = BindableProperty.Create(
            nameof(LongTappedCommand),
            typeof(ICommand),
            typeof(DrawPanel),
            default(ICommand),
            propertyChanged: (bindable, oldValue, newValue) => {
            });

        public void OnLongTapped(double x, double y)
        {
            if (LongTappedCommand != null && LongTappedCommand.CanExecute(new Point(x, y)))
                LongTappedCommand?.Execute(new Point(x, y));
        }

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
    }
}
