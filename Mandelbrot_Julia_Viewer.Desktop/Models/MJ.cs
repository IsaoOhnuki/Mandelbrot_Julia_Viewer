using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandelbrot_Julia_Viewer.Desktop.Models
{
    public class MJ
    {
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double IPos { get; set; }
        public double JPos { get; set; }
        public double Radius { get; set; }
        public short Repert { get; set; }
        public int Resolution { get; set; }

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

        public static Task<short[]> Mandelbrot(MJ mj)
        {
            return Task.Run<short[]>(() => {
                short[] data = new short[mj.Resolution * mj.Resolution];

#if ComplexClassDefine
                #region Complex Code
                Complex minimum = new Complex(mj.XPos - mj.Radius, mj.YPos - mj.Radius);
                Complex maximum = new Complex(mj.XPos + mj.Radius, mj.YPos + mj.Radius);
                #endregion
#else
                #region No Complex Code
                double xmin = mj.XPos - mj.Radius;
                double ymin = mj.YPos + mj.Radius;
                double ymax = mj.YPos + mj.Radius;
                #endregion
#endif
                int i = 0;
                for (; i < mj.Resolution; i++)
                {
#if ComplexClassDefine
                    #region Complex Code
                    double x = i * (maximum.Real - minimum.Real) / (mj.Resolution - 1);
                    #endregion
#else
                    #region No Complex Code
                    double x = i * mj.Radius * 2 / (mj.Resolution - 1) + xmin;
                    #endregion
#endif
                    for (int j = 0; j < mj.Resolution; j++)
                    {
                        int count = mj.Repert;

#if ComplexClassDefine
                        #region Complex Code
                        double y = j * (maximum.Imaginary - minimum.Imaginary) / (mj.Resolution - 1);
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
                        double y = ymax - j * mj.Radius * 2 / (mj.Resolution - 1);
                        //double y = j * mj.Radius * 2 / (mj.Resolution - 1) - ymin;
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
                        data[j * mj.Resolution + i] = (short)(mj.Repert - count);
                        if (data[j * mj.Resolution + i] == mj.Repert)
                            data[j * mj.Resolution + i] = 0;
                    }
                }
                for (; i < mj.Resolution; i++)
                {
                    for (int j = 0; j < mj.Resolution; j++)
                    {
                        data[j * mj.Resolution + i] = 0;
                    }
                }
                return data;
            });
        }

        public static Task<short[]> Julia(MJ mj)
        {
            return Task.Run<short[]>(() => {
                short[] data = new short[mj.Resolution * mj.Resolution];

#if ComplexClassDefine
                #region Complex Code
                Complex minimum = new Complex(mj.XPos - mj.Radius, mj.YPos - mj.Radius);
                Complex maximum = new Complex(mj.XPos + mj.Radius, mj.YPos + mj.Radius);
                Complex julia = new Complex(ipos, jpos);
                #endregion
#else
                #region No Complex Code
                double xmin = mj.XPos - mj.Radius;
                double ymin = mj.YPos + mj.Radius;
                double ymax = mj.YPos + mj.Radius;
                #endregion
#endif
                int i = 0;
                for (; i < mj.Resolution; i++)
                {
#if ComplexClassDefine
                    #region Complex Code
                    double x = i * (maximum.Real - minimum.Real) / (mj.Resolution - 1);
                    #endregion
#else
                    #region No Complex Code
                    double x = i * mj.Radius * 2 / (mj.Resolution - 1) + xmin;
                    #endregion
#endif
                    for (int j = 0; j < mj.Resolution; j++)
                    {
                        int count = mj.Repert;

#if ComplexClassDefine
                        #region Complex Code
                        double y = j * (maximum.Imaginary - minimum.Imaginary) / (mj.Resolution - 1);
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
                        //double y = j * mj.Radius * 2 / (mj.Resolution - 1) - ymin;
                        double y = ymax - j * mj.Radius * 2 / (mj.Resolution - 1);
                        double a = x;
                        double b = y;
                        double a2 = a * a;
                        double b2 = b * b;
                        while (a2 + b2 <= 4 && count > 0)
                        {
                            b = 2 * a * b + mj.JPos;
                            a = a2 - b2 + mj.IPos;
                            a2 = a * a;
                            b2 = b * b;
                            --count;
                        }
                        #endregion
#endif
                        data[j * mj.Resolution + i] = (short)(mj.Repert - count);
                        if (data[j * mj.Resolution + i] == mj.Repert)
                            data[j * mj.Resolution + i] = 0;
                    }
                }
                for (; i < mj.Resolution; i++)
                {
                    for (int j = 0; j < mj.Resolution; j++)
                    {
                        data[j * mj.Resolution + i] = 0;
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

        public static Task<short[]> JuliaMap(MJ mj, int split)
        {
            return Task.Run<short[]>(() => {
                int size = mj.Resolution / split;
                mj.Resolution = size * split;

                short[] data = new short[mj.Resolution * mj.Resolution];

                double julia = mj.Radius * 2 / (split - 1);

                List<Task<short[]>> tasks = new List<Task<short[]>>();

                for (int j = 0; j < split; ++j)
                {
                    for (int i = 0; i < split; ++i)
                    {
                        MJ _mj = new MJ { IPos = -mj.Radius + julia * i, JPos = mj.Radius - julia * j, XPos = 0, YPos = 0, Radius = 2, Repert = mj.Repert, Resolution = size };
                        tasks.Add(Julia(_mj));
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
                                data[j * size * mj.Resolution + i * size + y * mj.Resolution + x] = tasks[j * split + i].Result[y * size + x];
                            }
                        }
                    }
                }

                return data;
            });
        }
    }
}
