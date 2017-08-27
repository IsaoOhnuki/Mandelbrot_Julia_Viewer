using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Mandelbrot_Julia
    {
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double IPos { get; set; }
        public double JPos { get; set; }
        public double Radius { get; set; }
        public int Repert { get; set; }
        public int Resolution { get; set; }
        private int[] data;
        public int[] Data
        {
            get { return data; }
            set
            {
                data = null;
                if (value != null)
                {
                    data = new int[value.GetLength(0)];
                    value.CopyTo(data, 0);
                }
            }
        }
        private byte[] image;
        public byte[] Image
        {
            get { return image; }
            set
            {
                image = null;
                if (value != null)
                {
                    image = new byte[value.GetLength(0)];
                    value.CopyTo(image, 0);
                }
            }
        }
        public Mandelbrot_Julia(Mandelbrot_Julia mj)
        {
            IPos = mj.IPos;
            JPos = mj.JPos;
            XPos = mj.XPos;
            YPos = mj.YPos;
            Radius = mj.Radius;
            Repert = mj.Repert;
            Resolution = mj.Resolution;
            Data = mj.Data;
            Image = mj.Image;
        }
        public Mandelbrot_Julia(double xpos, double ypos, double radius, int repert, int resolution)
        {
            IPos = double.NaN;
            JPos = double.NaN;
            XPos = xpos;
            YPos = ypos;
            Radius = radius;
            Repert = repert;
            Resolution = resolution;
        }
        public Mandelbrot_Julia(double ipos, double jpos, double xpos, double ypos, double radius, int repert, int resolution)
        {
            IPos = ipos;
            JPos = jpos;
            XPos = xpos;
            YPos = ypos;
            Radius = radius;
            Repert = repert;
            Resolution = resolution;
        }

        public static bool operator ==(Mandelbrot_Julia l, Mandelbrot_Julia r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(Mandelbrot_Julia l, Mandelbrot_Julia r)
        {
            return !l.Equals(r);
        }

        public override bool Equals(object obj)
        {
            return (Double.IsNaN(IPos) ? Double.IsNaN(((Mandelbrot_Julia)obj).IPos) : IPos == ((Mandelbrot_Julia)obj).IPos)
                && (Double.IsNaN(JPos) ? Double.IsNaN(((Mandelbrot_Julia)obj).JPos) : JPos == ((Mandelbrot_Julia)obj).JPos)
                && (Double.IsNaN(XPos) ? Double.IsNaN(((Mandelbrot_Julia)obj).XPos) : XPos == ((Mandelbrot_Julia)obj).XPos)
                && (Double.IsNaN(YPos) ? Double.IsNaN(((Mandelbrot_Julia)obj).YPos) : YPos == ((Mandelbrot_Julia)obj).YPos)
                && (Double.IsNaN(Radius) ? Double.IsNaN(((Mandelbrot_Julia)obj).Radius) : Radius == ((Mandelbrot_Julia)obj).Radius)
                && Repert == ((Mandelbrot_Julia)obj).Repert
                && Resolution == ((Mandelbrot_Julia)obj).Resolution;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Task<byte[]> Develop(int repert, int[] data)
        {
            return
            Task.Run<byte[]>(() =>
            {
                byte[] image = new byte[data.GetLength(0) * 4];
                int dataIndex = 0;
                foreach (int d in data)
                {
                    image[dataIndex++] = (byte)(256.0 * d / repert);
                    image[dataIndex++] = (byte)(256.0 * d * 2.0 / repert);
                    image[dataIndex++] = (byte)(256.0 * d * 4.0 / repert);
                    image[dataIndex++] = 255;
                }
                return image;
            });
        }

        public static Task<int[]> Mandelbrot(double xpos, double ypos, double radius, int repert, int resolution)
        {
            return Task.Run<int[]>(() => {
                int[] data = new int[resolution * resolution];

                #region Complex Code
                //Complex minimum = new Complex(xpos - radius, ypos - radius);
                //Complex maximum = new Complex(xpos + radius, ypos + radius);
                #endregion

                #region No Complex Code
                double xmin = xpos - radius;
                double ymin = ypos + radius;
                #endregion

                int i = 0;
                for (; i < resolution; i++)
                {
                    #region Complex Code
                    //double x = i * (maximum.Real - minimum.Real) / (resolution - 1);
                    #endregion

                    #region No Complex Code
                    double x = i * radius * 2 / (resolution - 1) + xmin;
                    #endregion

                    for (int j = 0; j < resolution; j++)
                    {
                        int count = repert;

                        #region Complex Code
                        //double y = j * (maximum.Imaginary - minimum.Imaginary) / (resolution - 1);
                        //Complex c = new Complex(minimum.Real + x, maximum.Imaginary - y);
                        //Complex z = new Complex(c.Real, c.Imaginary);
                        //while (z.Magnitude < 2 && count > 0)
                        //{
                        //    z = (z * z) + c;
                        //    --count;
                        //}
                        #endregion

                        #region No Complex Code
                        double y = j * radius * 2 / (resolution - 1) - ymin;
                        double a = x;
                        double b = y;
                        double a2 = a * a;
                        double b2 = b * b;
                        while (a2 + b2 <= 4 && count > 0)
                        {
                            b = 2 * a * b + y;
                            a = a2 - b2 + x;
                            a2 = a * a;
                            b2 = b * b;
                            --count;
                        }
                        #endregion

                        data[j * resolution + i] = repert - count;
                    }
                }
                for (; i < resolution; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        data[j * resolution + i] = 0;
                    }
                }
                return data;
            });
        }

        public static Task<int[]> Julia(double ipos, double jpos, double xpos, double ypos, double radius, int repert, int resolution)
        {
            return Task.Run<int[]>(() => {
                int[] data = new int[resolution * resolution];

                #region Complex Code
                //Complex minimum = new Complex(xpos - radius, ypos - radius);
                //Complex maximum = new Complex(xpos + radius, ypos + radius);
                //Complex julia = new Complex(ipos, jpos);
                #endregion

                #region No Complex Code
                double xmin = xpos - radius;
                double ymax = ypos + radius;
                #endregion

                int i = 0;
                for (; i < resolution; i++)
                {
                    #region Complex Code
                    //double x = i * (maximum.Real - minimum.Real) / (resolution - 1);
                    #endregion

                    #region No Complex Code
                    double x = xmin + i * radius * 2 / (resolution - 1);
                    #endregion

                    for (int j = 0; j < resolution; j++)
                    {
                        int count = repert;

                        #region Complex Code
                        //double y = j * (maximum.Imaginary - minimum.Imaginary) / (resolution - 1);
                        //Complex z = new Complex(minimum.Real + x, maximum.Imaginary - y);
                        //while (z.Magnitude < 2 && count > 0)
                        //{
                        //    z = (z * z) + julia;
                        //    --count;
                        //}
                        #endregion

                        #region No Complex Code
                        double y = ymax - j * radius * 2 / (resolution - 1);
                        double a = x;
                        double b = y;
                        double a2 = a * a;
                        double b2 = b * b;
                        while (a2 + b2 <= 4 && count > 0)
                        {
                            b = 2 * a * b + jpos;
                            a = a2 - b2 + ipos;
                            a2 = a * a;
                            b2 = b * b;
                            --count;
                        }
                        #endregion

                        data[j * resolution + i] = repert - count;
                    }
                }
                for (; i < resolution; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        data[j * resolution + i] = 0;
                    }
                }
                return data;
            });
        }
    }
}
