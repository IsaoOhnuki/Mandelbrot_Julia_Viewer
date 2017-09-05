using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Controls
{
    public class GradationDrawer : View
    {
        public class ColPos
        {
            public Color Color { get; set; }
            public double Position { get; set; }
        }

        public ObservableCollection<ColPos> Colors { get; protected set; }

        public GradationDrawer()
        {
            Colors = new ObservableCollection<ColPos>();
        }

        public void Add(Color color, int position)
        {
            Colors.Add(new ColPos { Color = color, Position = position });
        }

        public void Insert(int index, Color color, int position)
        {
            Colors.Insert(index, new ColPos { Color = color, Position = position });
        }

        public void Remove(int index)
        {
            Colors.RemoveAt(index);
        }

        public Color[] CreateColorArray(int arraySize)
        {
            Color[] ret = new Color[arraySize + 1];
            for (int arrayidx = 0; arrayidx < arraySize; ++arrayidx)
            {
                double pos = (double)arrayidx / arraySize;
                int prevcolpos = 0;
                for (int colpos = 1; colpos < Colors.Count; ++colpos)
                {
                    if (pos > Colors[prevcolpos].Position && pos <= Colors[colpos].Position)
                    {
                        double r = (Colors[colpos].Color.R - Colors[prevcolpos].Color.R) * pos / (Colors[colpos].Position - Colors[prevcolpos].Position) + Colors[prevcolpos].Color.R;
                        double g = (Colors[colpos].Color.G - Colors[prevcolpos].Color.G) * pos / (Colors[colpos].Position - Colors[prevcolpos].Position) + Colors[prevcolpos].Color.G;
                        double b = (Colors[colpos].Color.B - Colors[prevcolpos].Color.B) * pos / (Colors[colpos].Position - Colors[prevcolpos].Position) + Colors[prevcolpos].Color.B;
                        ret[arrayidx] = new Color(r, g, b);
                        break;
                    }
                }
            }
            return ret;
        }
    }
}
