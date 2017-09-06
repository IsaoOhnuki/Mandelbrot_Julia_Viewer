using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Models
{
    public class ColorResolution
    {
        public ColorResolutionStruct[] ColorResolutions { get; set; }
        public Color[] Colors { get; set; }
        public ColorResolution(ColorResolution parette)
        {
            if (parette as object == null || parette.Colors == null)
                return;
            Colors = new Color[parette.Colors.GetLength(0)];
            parette.Colors.CopyTo(Colors, 0);
            ColorResolutions = new ColorResolutionStruct[parette.ColorResolutions.GetLength(0)];
            parette.ColorResolutions.CopyTo(ColorResolutions, 0);
        }
        public ColorResolution(int repert, ColorResolutionStruct[] colorResolutions)
        {
            if (colorResolutions as object == null)
                return;
            Colors = Parette(repert, colorResolutions);
            ColorResolutions = new ColorResolutionStruct[colorResolutions.GetLength(0)];
            colorResolutions.CopyTo(ColorResolutions, 0);
        }
        public static implicit operator Color[] (ColorResolution parette)
        {
            if (parette as object == null || parette.Colors == null)
                return null;
            Color[] ret = new Color[parette.Colors.GetLength(0)];
            parette.Colors.CopyTo(ret, 0);
            return ret;
        }
        // http://www.sofgate.com/design/ct_gradation.html グラデーション配色の計算方法
        public static Color[] Parette(int repert, ColorResolutionStruct[] cols)
        {
            Color[] ret = new Color[repert + 1];
            for (int arrayidx = 0; arrayidx < repert; ++arrayidx)
            {
                double pos = (double)arrayidx / (repert - 1);
                int prevcolpos = 0;
                for (int colpos = 1; colpos < cols.Count(); ++colpos)
                {
                    if (pos > cols[prevcolpos].Position && pos <= cols[colpos].Position)
                    {
                        double r = (cols[colpos].Color.R - cols[prevcolpos].Color.R) * pos / (cols[colpos].Position - cols[prevcolpos].Position) + cols[prevcolpos].Color.R;
                        double g = (cols[colpos].Color.G - cols[prevcolpos].Color.G) * pos / (cols[colpos].Position - cols[prevcolpos].Position) + cols[prevcolpos].Color.G;
                        double b = (cols[colpos].Color.B - cols[prevcolpos].Color.B) * pos / (cols[colpos].Position - cols[prevcolpos].Position) + cols[prevcolpos].Color.B;
                        ret[arrayidx] = new Color(r, g, b);
                        break;
                    }
                }
            }
            ret[repert] = ret[0];
            return ret;
        }
        public static bool operator ==(ColorResolution l, ColorResolution r)
        {
            if (l as object == null || r as object == null || l.Colors == null || r.Colors == null)
                return false;
            bool ret = l.Colors.GetLength(0) == r.Colors.GetLength(0);
            for (int idx = 0; ret && idx < l.Colors.GetLength(0); ++idx)
            {
                ret = l.Colors[idx] == r.Colors[idx];
            }
            return ret;
        }
        public static bool operator !=(ColorResolution l, ColorResolution r)
        {
            if (l as object == null || r as object == null || l.Colors == null || r.Colors == null)
                return true;
            bool ret = l.Colors.GetLength(0) != r.Colors.GetLength(0);
            for (int idx = 0; !ret && idx < l.Colors.GetLength(0); ++idx)
            {
                ret = l.Colors[idx] != r.Colors[idx];
            }
            return ret;
        }
        public override bool Equals(object obj)
        {
            return this == obj as ColorResolution;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct ColorResolutionStruct
    {
        public Color Color { get; set; }
        public double Position { get; set; }
    }
}
