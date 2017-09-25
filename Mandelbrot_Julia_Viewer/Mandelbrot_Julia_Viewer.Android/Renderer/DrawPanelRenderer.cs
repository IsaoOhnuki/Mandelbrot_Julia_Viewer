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

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.Droid.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.Droid
{
    // http://serenegiant.com/blog/?p=15 ImageViewを作ってみた〜その１〜準備編
    // http://qiita.com/AyaseSH/items/52768d4a9f22f417642f Xamarin.FormsでPinchGestureRecognizerのユーザー操作について
    // http://ticktack.hatenablog.jp/entry/2016/06/11/124751 【Xamarin.Forms】ViewRendererと仲良くなるための簡易チュートリアル
    // http://seesaawiki.jp/w/moonlight_aska/d/%CA%A3%BB%A8%A4%CA%A5%BF%A5%C3%A5%C1%A5%A4%A5%D9%A5%F3%A5%C8%A4%F2%BC%E8%C6%C0%A4%B9%A4%EB 複雑なタッチイベントを取得する
    // http://qiita.com/bassyaroo/items/ed13b2da3b289faa0d89 Android ピンチイン　ピンチアウト　ロングプレス　併用する

    class DrawPanelRenderer : ViewRenderer<DrawPanel, Android.Views.View>
    {
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
                //ctrl.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All;
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                //Control.Draw -= Control_Draw;
                //Control.SizeChanged -= Control_SizeChanged;
                //Control.ManipulationDelta -= Control_ManipulationDelta;
                //Control.PointerWheelChanged -= Control_PointerWheelChanged;

                Element.ImageCompile -= ImageCompile;
            }
            if (Control != null && e.NewElement != null)
            {
                //Control.Draw += Control_Draw;
                //Control.SizeChanged += Control_SizeChanged;
                //Control.ManipulationDelta += Control_ManipulationDelta;
                //Control.PointerWheelChanged += Control_PointerWheelChanged;

                Element.ImageCompile += ImageCompile;
            }
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "DeviceImage")
            {
                //DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

                Control.Invalidate();
            }
        }

        Task<object> ImageCompile(byte[] image, int x, int y, int p)
        {
            return Task.Run(() => {
                return (object)null;
                //return (object)Android.Graphics.BitmapFactory.DecodeByteArray(image, x, y, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96);
            });
        }

        //private async void Control_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        //{
        //    //Debug.WriteLine("Control_SizeChanged");

        //    DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

        //    Control.Invalidate();
        //}

        //private async void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    //Debug.WriteLine("Control_PointerWheelChanged");
        //    var pointer = e.GetCurrentPoint(Control).Properties;
        //    if (!pointer.IsLeftButtonPressed && !pointer.IsRightButtonPressed)
        //    {
        //        int delta = pointer.MouseWheelDelta;
        //        Xamarin.Forms.Point offset = new Xamarin.Forms.Point();
        //        if (delta > 0)
        //        {
        //            Scale = Scale * (0.2 * delta / 120 + 1);
        //            //offset.X = (ViewSize.Width / Scale - ViewSize.Width) / 2;
        //            //offset.Y = (ViewSize.Height / Scale - ViewSize.Height) / 2;
        //        }
        //        else if (delta < 0)
        //        {
        //            Scale = Scale / (0.2 * -delta / 120 + 1);
        //            //offset.X = (ViewSize.Width * Scale - ViewSize.Width) / 2;
        //            //offset.Y = (ViewSize.Height * Scale - ViewSize.Height) / 2;
        //        }

        //        //offset.X = pointer.ContactRect.X;
        //        //offset.Y = pointer.ContactRect.Y;

        //        ViewPoint = ViewPoint.Offset(offset.X, offset.Y);

        //        DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

        //        Control.Invalidate();
        //    }
        //}

        //private async void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        //{
        //    //Debug.WriteLine("Control_ManipulationDelta");
        //    ViewPoint = ViewPoint.Offset(-e.Delta.Translation.X / Scale, -e.Delta.Translation.Y / Scale);

        //    DrawImage = await Element.DrawImmageRequestAsync(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

        //    Control.Invalidate();
        //}

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            Paint paint = new Paint(PaintFlags.AntiAlias);

            paint.Color = new Android.Graphics.Color((byte)(Element.BackgroundColor.A * 255d), (byte)(Element.BackgroundColor.R * 255d), (byte)(Element.BackgroundColor.G * 255d), (byte)(Element.BackgroundColor.B * 255d));
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new Rect(0, 0, Width, Height), paint);


            if (DrawImage != null && DrawImage.Image != null)
            {
                Rectangle reqRect = new Rectangle(ViewPoint, Matrix2.Enlargement(ViewSize, 1 / Scale, 1 / Scale));

                //Rectangle viewRect = DrawImage.DrawRect.Offset(-ViewPoint.X, -ViewPoint.Y);
                Rectangle viewRect = new Rectangle
                {
                    Location = Matrix2.Enlargement(DrawImage.DrawRect.Location.Offset(-ViewPoint.X, -ViewPoint.Y), Scale, Scale),
                    Size = Matrix2.Enlargement(DrawImage.DrawRect.Size, Scale, Scale)
                };

                //args.DrawingSession.DrawImage(DrawImage.Image as ICanvasImage, GetDeviceRect(viewRect), GetDeviceRect(DrawImage.DrawRect));
            }
        }

        public Rect GetDeviceRect(Rectangle rect)
        {
            return new Rect((int)rect.X, (int)rect.Y, (int)rect.Right, (int)rect.Bottom);
        }
    }

}
