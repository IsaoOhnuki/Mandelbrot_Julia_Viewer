using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Controls
{
    public class GradationDrawer : View
    {
        public class ColPos : BindableObject
        {
            private Color color;
            public Color Color
            {
                get { return color; }
                set
                {
                    color = value;
                    OnPropertyChanged();
                }
            }
            private int position;
            public int Position
            {
                get { return position; }
                set
                {
                    position = value;
                    OnPropertyChanged();
                }
            }
        }

        private IList<ColPos> colors { get { return Colors as IList<ColPos>; } }

        public object Colors
        {
            get { return GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        // BindablePropertyを追加
        public static readonly BindableProperty ColorsProperty = BindableProperty.Create(
            nameof(Colors),
            typeof(object),
            typeof(GradationDrawer),
            default(object),
            propertyChanged: (bindable, oldValue, newValue) => {
                if (oldValue is IList<ColPos> && oldValue is INotifyCollectionChanged)
                {
                    foreach (var val in oldValue as IList<ColPos>)
                    {
                        val.PropertyChanged -= GradationDrawer_PropertyChanged;
                    }
                    ((INotifyCollectionChanged)oldValue).CollectionChanged -= GradationDrawer_CollectionChanged;
                }
                if (newValue is IList<ColPos> && newValue is INotifyCollectionChanged)
                {
                    foreach (var val in newValue as IList<ColPos>)
                    {
                        val.PropertyChanged += GradationDrawer_PropertyChanged;
                    }
                    ((INotifyCollectionChanged)newValue).CollectionChanged += GradationDrawer_CollectionChanged;
                }
            });

        private static void GradationDrawer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private static void GradationDrawer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public GradationDrawer()
        {
        }

        public Color[] CreateColorArray(int arraySize)
        {
            Color[] ret = new Color[arraySize];
            if (colors != null)
            {
                for (int arrayidx = 0; arrayidx < arraySize; ++arrayidx)
                {
                    double pos = (double)arrayidx * 100 / arraySize;
                    for (int prevcolpos = 0, colpos = 1; colpos < colors.Count; ++colpos, ++prevcolpos)
                    {
                        double prevPosition = colors[prevcolpos].Position;
                        double position = colors[colpos].Position;
                        if (pos >= prevPosition && (pos < position || colpos == colors.Count - 1))
                        {
                            double r = (colors[colpos].Color.R - colors[prevcolpos].Color.R) * (pos - prevPosition) / (position - prevPosition) + colors[prevcolpos].Color.R;
                            double g = (colors[colpos].Color.G - colors[prevcolpos].Color.G) * (pos - prevPosition) / (position - prevPosition) + colors[prevcolpos].Color.G;
                            double b = (colors[colpos].Color.B - colors[prevcolpos].Color.B) * (pos - prevPosition) / (position - prevPosition) + colors[prevcolpos].Color.B;
                            ret[arrayidx] = new Color(r, g, b);
                            break;
                        }
                    }
                }
            }
            return ret;
        }
    }
}
