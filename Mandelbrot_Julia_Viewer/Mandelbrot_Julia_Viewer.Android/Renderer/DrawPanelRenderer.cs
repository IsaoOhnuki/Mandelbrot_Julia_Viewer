using Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using static Controls.DrawPanel;
using Android.Graphics;
using static Android.Views.ScaleGestureDetector;
using static Android.Views.GestureDetector;
using Models;
using Android.Widget;

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.Droid.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    // http://serenegiant.com/blog/?p=15 ImageViewを作ってみた〜その１〜準備編
    // http://qiita.com/AyaseSH/items/52768d4a9f22f417642f Xamarin.FormsでPinchGestureRecognizerのユーザー操作について
    // http://ticktack.hatenablog.jp/entry/2016/06/11/124751 【Xamarin.Forms】ViewRendererと仲良くなるための簡易チュートリアル
    // http://seesaawiki.jp/w/moonlight_aska/d/%ca%a3%bb%a8%a4%ca%a5%bf%a5%c3%a5%c1%a5%a4%a5%d9%a5%f3%a5%c8%a4%f2%bc%e8%c6%c0%a4%b9%a4%eb 複雑なタッチイベントを取得する
    // http://qiita.com/bassyaroo/items/ed13b2da3b289faa0d89 Android ピンチイン　ピンチアウト　ロングプレス　併用する
    // http://zawapro.com/?p=1474 【Android】GestuerDetectorとScrollerを組み合わせた例
    // https://qiita.com/shinido/items/65399846a5e9eba1aa5e GestureDetectorのまとめ

    class DrawPanelRenderer : ViewRenderer<DrawPanel, Android.Views.View>, IOnScaleGestureListener, IOnGestureListener
    {
        public ScaleGestureDetector ScaleGestureDetector { get; set; }
        public GestureDetector GestureDetector { get; set; }
        public Scroller Scroller { get; set; }

        public Xamarin.Forms.Point ViewPoint { get; set; }
        public Size ViewSize { get { return new Size(Control.Width, Control.Height); } }
        public Xamarin.Forms.Point ViewOrigin { get { return new Xamarin.Forms.Point(Control.Width / 2, Control.Height / 2); } }
        private double scale = 1;
        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                if (scale < 0.001)
                    scale = 0.001;
            }
        }

        public DrawImageStruct DrawImage { get; set; }

        protected override void OnElementChanged(ElementChangedEventArgs<DrawPanel> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new Android.Views.View(this.Context);

                ScaleGestureDetector = new ScaleGestureDetector(this.Context, this);
                GestureDetector = new GestureDetector(this.Context, this);
                GestureDetector.IsLongpressEnabled = false;
                Scroller = new Scroller(this.Context);

                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                Control.Touch -= Control_Touch;
                Control.LayoutChange -= Control_LayoutChange;

                Element.ImageCompile -= ImageCompile;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Touch += Control_Touch;
                Control.LayoutChange += Control_LayoutChange;

                Element.ImageCompile += ImageCompile;
            }
            base.OnElementChanged(e);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "DeviceImage")
            {
                DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

                Invalidate();
            }
        }

        Task<object> ImageCompile(byte[] image, int x, int y, int p)
        {
            return Task.Run(async() => {
                byte[] bmpData = await BitmapCreator.Create((short)x, (short)y, image, false);
                return (object)BitmapFactory.DecodeByteArray(bmpData, 0, bmpData.Length);
            });
        }

        private void Control_LayoutChange(object sender, LayoutChangeEventArgs e)
        {
            //Debug.WriteLine("Control_SizeChanged");

            DrawImage = Element.DrawImmageRequest(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

            Invalidate();
        }

        private void Control_Touch(object sender, TouchEventArgs e)
        {
            if (e.Event.PointerCount == 1)
                GestureDetector.OnTouchEvent(e.Event);
            else
                ScaleGestureDetector.OnTouchEvent(e.Event);
        }

        public bool OnScale(ScaleGestureDetector detector)
        {
            //Debug.WriteLine("Control_PointerWheelChanged");

            if (detector.PreviousSpan != detector.CurrentSpan)
            {
                double oldScale = Scale;
                double sclae = detector.PreviousSpan / detector.CurrentSpan;
                if (sclae > 1)
                {
                    Scale = Scale * (sclae * 1.2);
                }
                else if (sclae < 1)
                {
                    Scale = Scale * (sclae / 1.2);
                }

                Xamarin.Forms.Point oldPoint = ViewPoint.Offset(detector.FocusX / oldScale, detector.FocusY / oldScale);
                Xamarin.Forms.Point newPoint = ViewPoint.Offset(detector.FocusX / Scale, detector.FocusY / Scale);
                ViewPoint = ViewPoint.Offset(oldPoint.X - newPoint.X, oldPoint.Y - newPoint.Y);

                DrawImage = Element.DrawImmageRequest(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

                Invalidate();
            }
            return true;
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            //throw new NotImplementedException();
        }

        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            Scroller.Fling((int)e2.GetX(), (int)e2.GetY(), (int)velocityX, (int)velocityY, 0, Width, 0, Height);
            //ViewPoint = ViewPoint.Offset(-velocityX / Scale, -velocityY / Scale);

            //DrawImage = Element.DrawImmageRequest(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

            //Invalidate();
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
            //IsLongpressEnabled = true
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            //Debug.WriteLine("Control_ManipulationDelta");

            ViewPoint = ViewPoint.Offset(-distanceX, -distanceY);

            DrawImage = Element.DrawImmageRequest(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

            Invalidate();
            return true;
        }

        public void OnShowPress(MotionEvent e)
        {
            //throw new NotImplementedException();
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            Paint paint = new Paint(PaintFlags.AntiAlias);

            paint.Color = new Android.Graphics.Color((byte)(Element.BackgroundColor.A * 255d), (byte)(Element.BackgroundColor.R * 255d), (byte)(Element.BackgroundColor.G * 255d), (byte)(Element.BackgroundColor.B * 255d));
            //paint.Color = Android.Graphics.Color.Honeydew;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new Rect(0, 0, Width, Height), paint);


            if (DrawImage != null && DrawImage.Image != null)
            {
                Rectangle viewRect = new Rectangle
                {
                    Location = Matrix2.Enlargement(DrawImage.DrawRect.Location.Offset(-ViewPoint.X, -ViewPoint.Y), Scale, Scale),
                    Size = Matrix2.Enlargement(DrawImage.DrawRect.Size, Scale, Scale)
                };

                canvas.DrawBitmap((Bitmap)DrawImage.Image, GetDeviceRect(viewRect), GetDeviceRect(DrawImage.DrawRect), null);
            }
        }

        public Rect GetDeviceRect(Rectangle rect)
        {
            return new Rect((int)rect.X, (int)rect.Y, (int)rect.Right, (int)rect.Bottom);
        }
    }

}
