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
            Color prevCol = Color.Transparent;
            int prevPos = 0;
            foreach (var col in Colors)
            {
                int pos = (int)((arraySize - 1) * col.Position);
                ret[pos] = col.Color;
                if (prevCol != Color.Transparent)
                {
                    int dist = pos - prevPos + 1;
                    if (dist > 2)
                    {
                        for (int graPos = prevPos + 1; graPos < pos; ++graPos)
                        {
                            double r = (ret[pos].R - prevCol.R) * (graPos - prevPos) / dist + prevCol.R;
                            double g = (ret[pos].G - prevCol.G) * (graPos - prevPos) / dist + prevCol.G;
                            double b = (ret[pos].B - prevCol.B) * (graPos - prevPos) / dist + prevCol.B;
                            ret[graPos] = new Color(r, g, b);
                        }
                    }
                }
                prevCol = col.Color;
                prevPos = pos;
            }
            return ret;
        }
    }
}
