//#define ComplexClassDefine

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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
        public int JuliaMapSplit { get; set; }
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
        public Mandelbrot_Julia(double xpos, double ypos, double radius, int repert, int resolution, int split, /*int colorParette, */ColorResolutionStruct[] colors)
        {
            IPos = double.NaN;
            JPos = double.NaN;
            XPos = xpos;
            YPos = ypos;
            Radius = radius;
            Repert = repert;
            Resolution = resolution;
            JuliaMapSplit = split;
        }
        public Mandelbrot_Julia(double xpos, double ypos, double radius, int repert, int resolution, /*int colorParette, */ColorResolutionStruct[] colors)
        {
            IPos = double.NaN;
            JPos = double.NaN;
            XPos = xpos;
            YPos = ypos;
            Radius = radius;
            Repert = repert;
            Resolution = resolution;
            JuliaMapSplit = 0;
        }
        public Mandelbrot_Julia(double ipos, double jpos, double xpos, double ypos, double radius, int repert, int resolution, /*int colorParette, */ColorResolutionStruct[] colors)
        {
            IPos = ipos;
            JPos = jpos;
            XPos = xpos;
            YPos = ypos;
            Radius = radius;
            Repert = repert;
            Resolution = resolution;
            JuliaMapSplit = 0;
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
                && Resolution == ((Mandelbrot_Julia)obj).Resolution
                && JuliaMapSplit == ((Mandelbrot_Julia)obj).JuliaMapSplit;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Task<byte[]> Develop(int repert, int[] data, Color[] cols = null)
        {
            return
            Task.Run<byte[]>(() =>
            {
                byte[] image = new byte[data.GetLength(0) * 4];
                int dataIndex = 0;
                if (cols == null)
                {
                    foreach (int d in data)
                    {
                        image[dataIndex++] = (byte)(256.0 * d / repert);
                        image[dataIndex++] = (byte)(256.0 * d * 2.0 / repert);
                        image[dataIndex++] = (byte)(256.0 * d * 4.0 / repert);
                        image[dataIndex++] = 255;
                    }
                }
                else
                {
                    foreach (int d in data)
                    {
                        image[dataIndex++] = (byte)(255.0 * cols[d].B);
                        image[dataIndex++] = (byte)(255.0 * cols[d].G);
                        image[dataIndex++] = (byte)(255.0 * cols[d].R);
                        image[dataIndex++] = 255;
                    }
                }
                return image;
            });
        }

#if ComplexClassDefine
        private class Complex
        {
            public double Real { get; set; }
            public double Imaginary { get; set; }
            public double Magnitude { get { return Math.Sqrt(Real * Real + Imaginary * Imaginary); } }
            public double Phase { get { return Math.Atan2(Imaginary, Real); } }

            public Complex(double real, double imaginary)
            {
                Real = real;
                Imaginary = imaginary;
            }

            public static Complex operator +(Complex l, Complex r)
            {
                return new Complex(l.Real + r.Real, l.Imaginary + r.Imaginary);
            }

            public static Complex operator *(Complex l, Complex r)
            {
                return new Complex(l.Real * r.Real - l.Imaginary * r.Imaginary, l.Real * r.Imaginary + r.Real * l.Imaginary);
            }
        }
#endif

        public static Task<int[]> Mandelbrot(double xpos, double ypos, double radius, int repert, int resolution)
        {
            return Task.Run<int[]>(() => {
                int[] data = new int[resolution * resolution];

#if ComplexClassDefine
                #region Complex Code
                Complex minimum = new Complex(xpos - radius, ypos - radius);
                Complex maximum = new Complex(xpos + radius, ypos + radius);
                #endregion
#else
                #region No Complex Code
                double xmin = xpos - radius;
                double ymin = ypos + radius;
                #endregion
#endif
                int i = 0;
                for (; i < resolution; i++)
                {
#if ComplexClassDefine
                    #region Complex Code
                    double x = i * (maximum.Real - minimum.Real) / (resolution - 1);
                    #endregion
#else
                    #region No Complex Code
                    double x = i * radius * 2 / (resolution - 1) + xmin;
                    #endregion
#endif
                    for (int j = 0; j < resolution; j++)
                    {
                        int count = repert;

#if ComplexClassDefine
                        #region Complex Code
                        double y = j * (maximum.Imaginary - minimum.Imaginary) / (resolution - 1);
                        Complex c = new Complex(minimum.Real + x, maximum.Imaginary - y);
                        Complex z = new Complex(c.Real, c.Imaginary);
                        while ((z.Real * z.Real + z.Imaginary * z.Imaginary) <= 4 && count > 0)
                        //while (z.Magnitude < 2 && count > 0)
                        {
                            z = (z * z) + c;
                            --count;
                        }
                        #endregion
#else
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
#endif
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

#if ComplexClassDefine
                #region Complex Code
                Complex minimum = new Complex(xpos - radius, ypos - radius);
                Complex maximum = new Complex(xpos + radius, ypos + radius);
                Complex julia = new Complex(ipos, jpos);
                #endregion
#else
                #region No Complex Code
                double xmin = xpos - radius;
                double ymin = ypos + radius;
                #endregion
#endif
                int i = 0;
                for (; i < resolution; i++)
                {
#if ComplexClassDefine
                    #region Complex Code
                    double x = i * (maximum.Real - minimum.Real) / (resolution - 1);
                    #endregion
#else
                    #region No Complex Code
                    double x = i * radius * 2 / (resolution - 1) + xmin;
                    #endregion
#endif
                    for (int j = 0; j < resolution; j++)
                    {
                        int count = repert;

#if ComplexClassDefine
                        #region Complex Code
                        double y = j * (maximum.Imaginary - minimum.Imaginary) / (resolution - 1);
                        Complex z = new Complex(minimum.Real + x, maximum.Imaginary - y);
                        while ((z.Real * z.Real + z.Imaginary * z.Imaginary) <= 4 && count > 0)
                        //while (z.Magnitude < 2 && count > 0)
                        {
                            z = (z * z) + julia;
                            --count;
                        }
                        #endregion
#else
                        #region No Complex Code
                        double y = j * radius * 2 / (resolution - 1) - ymin;
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
#endif
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

        //http://qwerty2501.hatenablog.com/entry/2014/04/24/235849 async/await ～非同期なライブラリは楽じゃない～
        //http://mcommit.hatenadiary.com/entry/2016/07/22/014657 C# Task async, await の使い方
        //http://ufcpp.net/study/csharp/misc_task.html [雑記] スレッド プールとタスク
        //http://outside6.wp.xdomain.jp/2016/08/06/post-343/ Task.Waitはスレッドをロックする Task.Waitの問題点
        //http://outside6.wp.xdomain.jp/2016/08/09/post-568/ TaskとawaitのデッドロックをTaskで回避する
        //http://qiita.com/acple@github/items/8f63aacb13de9954c5da Taskを極めろ！async/await完全攻略

        public static Task<int[]> JuliaMap(double xpos, double ypos, double radius, int repert, int resolution, int split)
        {
            return Task.Run<int[]>(() => {
                int size = resolution / split;
                resolution = size * split;

                int[] data = new int[resolution * resolution];

                double julia = radius * 2 / (split - 1);

                List<Task<int[]>> tasks = new List<Task<int[]>>();

                for (int j = 0; j < split; ++j)
                {
                    for (int i = 0; i < split; ++i)
                    {
                        tasks.Add(Mandelbrot_Julia.Julia(-radius + julia * i, -radius + julia * j, 0, 0, 2, repert, size));
                    }
                }

                Task.WaitAll(tasks.ToArray());

                for (int j = 0; j < split; ++j)
                {
                    for (int i = 0; i < split; ++i)
                    {
                        for (int y = 0; y < size; ++y)
                        {
                            for (int x = 0; x < size; ++x)
                            {
                                data[j * size * resolution + i * size + y * resolution + x] = tasks[j * split + i].Result[y * size + x];
                            }
                        }
                    }
                }

                return data;
            });
        }
    }
}
