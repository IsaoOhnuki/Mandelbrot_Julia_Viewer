﻿using Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using System.ComponentModel;
using Windows.UI.Xaml.Markup;
using System.Xml;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

[assembly: ExportRenderer(typeof(GradationDrawer), typeof(Mandelbrot_Julia_Viewer.UWP.GradationDrawerRenderer))]
namespace Mandelbrot_Julia_Viewer.UWP
{
    // https://docs.microsoft.com/ja-jp/windows/uwp/controls-and-patterns/control-templates コントロール テンプレート
    // http://qiita.com/soi/items/12ceea4efcf31c1a7b93 シンプルなUserControl ～多分これが一番簡単だと思います
    // http://blog.okazuki.jp/entry/2016/03/01/072955 UWPで画像に文字を描いたりする

    class GradationDrawerRenderer : ViewRenderer<GradationDrawer, CanvasControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<GradationDrawer> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new CanvasControl();
                SetNativeControl(ctrl);
            }
            if (Control != null && e.OldElement != null)
            {
                if (e.OldElement.Colors is IList<GradationDrawer.ColPos>)
                {
                    foreach (var val in e.OldElement.Colors as IList<GradationDrawer.ColPos>)
                    {
                        val.PropertyChanged -= Colors_PropertyChanged;
                    }
                    Children.Clear();
                }
                if (e.OldElement.Colors is INotifyCollectionChanged)
                    (e.OldElement.Colors as INotifyCollectionChanged).CollectionChanged -= Colors_CollectionChanged;
                ((CanvasControl)Control).Tapped -= GradationDrawerRenderer_Tapped;
                ((CanvasControl)Control).SizeChanged -= GradationDrawerRenderer_SizeChanged;
                ((CanvasControl)Control).Draw -= GradationDrawerRenderer_Draw;
            }
            if (Control != null && e.NewElement != null)
            {
                if (e.NewElement.Colors is IList<GradationDrawer.ColPos>)
                {
                    foreach (var val in e.NewElement.Colors as IList<GradationDrawer.ColPos>)
                    {
                        val.PropertyChanged += Colors_PropertyChanged;
                        var picker = new ColorPicker();
                        picker.SelectedColor = val.Color;
                        Children.Insert(Children.Count, picker.GetOrCreateRenderer() as Panel);
                    }
                }
                if (e.NewElement.Colors is INotifyCollectionChanged)
                    (e.NewElement.Colors as INotifyCollectionChanged).CollectionChanged += Colors_CollectionChanged;
                ((CanvasControl)Control).Tapped += GradationDrawerRenderer_Tapped;
                ((CanvasControl)Control).SizeChanged += GradationDrawerRenderer_SizeChanged;
                ((CanvasControl)Control).Draw += GradationDrawerRenderer_Draw;
            }
            base.OnElementChanged(e);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            // プロパティ値の変更を反映
            if (e.PropertyName == GradationDrawer.ColorsProperty.PropertyName)
            {
                Gradation = await GetGradation((int)ActualHeight);
                Control.Invalidate();
            }
        }

        private async void Colors_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GradationDrawer.ColPos.Color))
            {

            }
            if (e.PropertyName == nameof(GradationDrawer.ColPos.Position))
            {

            }
            Gradation = await GetGradation((int)ActualHeight);
            Control.Invalidate();
        }

        private async void Colors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                int insertIndex = e.NewStartingIndex;
                foreach (var parette in e.NewItems)
                {
                    ((GradationDrawer.ColPos)parette).PropertyChanged += Colors_PropertyChanged;
                    var picker = new ColorPicker();
                    picker.SelectedColor = ((IList<GradationDrawer.ColPos>)Element.Colors)[insertIndex].Color;
                    Children.Insert(insertIndex++, picker.GetOrCreateRenderer() as Panel);
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                for (int l = 0; l < e.OldItems.Count; ++l)
                {
                    Children.RemoveAt(e.OldStartingIndex + l);
                }
            }
            Gradation = await GetGradation((int)ActualHeight);
            Control.Invalidate();
        }

        private async void GradationDrawerRenderer_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            Gradation = await GetGradation((int)e.NewSize.Height);
            Control.Invalidate();
        }

        private void GradationDrawerRenderer_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private void GradationDrawerRenderer_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.FillRectangle(new Windows.Foundation.Rect(0, 0, ActualWidth, ActualHeight), Windows.UI.Colors.Honeydew);
            for (int y = 0; y < Gradation?.Length; ++y)
            {
                args.DrawingSession.DrawLine(0, y, (int)ActualWidth / 2, y, Gradation[y]);
            }
        }

        public Windows.UI.Color[] Gradation { get; set; }

        private Task<Windows.UI.Color[]> GetGradation(int size)
        {
            return Task<Windows.UI.Color[]>.Run(() => {
                return Element.CreateColorArray(size).Select(x => new Windows.UI.Color { A = 255, R = (byte)(x.R * 255.0), G = (byte)(x.G * 255.0), B = (byte)(x.B * 255.0) }).ToArray();
            });
        }
    }
}
