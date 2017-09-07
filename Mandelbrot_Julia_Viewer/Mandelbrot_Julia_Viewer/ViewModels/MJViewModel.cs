using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mandelbrot_Julia_Viewer.ViewModels
{
    // http://qiita.com/kiichi54321/items/7b7a7b0caa2f09ba4147 UWPのアプリを作っているうえで、早く知りたかったこと。
    // http://garicchi.com/?p=16481 プリンターを使って印刷するには
    public class MJViewModel : BindableObject
    {
        public class MJI
        {
            public Mandelbrot_Julia MJ { get; set; }
            public ColorResolution ColorResolution { get; set; }
            public byte[] Bmp { get; set; }
        }

        public Command MandelbrotRun { get; set; }
        public Command JuliaRun { get; set; }
        public Command JuliaMupRun { get; set; }
        public Command Undo { get; set; }
        public Command Redo { get; set; }
        public bool CanUndo { get { return UndoList.CanUndo; } }
        public bool CanRedo { get { return UndoList.CanRedo; } }
        public UndoList<MJI> UndoList { get; set; }
        public string FractalType { get; set; }
        private bool useColorParette;
        public bool UseColorParette
        {
            get { return useColorParette; }
            set
            {
                useColorParette = value;
                OnPropertyChanged();
            }
        }
        private ColorResolutionStruct[] colorParette;
        public ColorResolutionStruct[] ColorParette
        {
            get { return colorParette; }
            set
            {
                //colorParette = value;
                colorParette = null;
                if (value != null)
                {
                    colorParette = new ColorResolutionStruct[value.GetLength(0)];
                    value.CopyTo(colorParette, 0);
                }
                OnPropertyChanged();
            }
        }
        private Rectangle imageBound;
        public Rectangle ImageBound
        {
            get { return imageBound; }
            set
            {
                imageBound = value;
                OnPropertyChanged();
            }
        }
        private double rate;
        public double Rate
        {
            get { return rate; }
            set
            {
                rate = value;
                OnPropertyChanged();
                ImageBound = new Rectangle(0, 0, Rate * ImageWidth, Rate * ImageHeight);
            }
        }
        private int imageWidth;
        public int ImageWidth
        {
            get { return imageWidth; }
            set
            {
                imageWidth = value;
                OnPropertyChanged();
                ImageBound = new Rectangle(0, 0, Rate * ImageWidth, Rate * ImageHeight);
            }
        }
        private int imageHeight;
        public int ImageHeight
        {
            get { return imageHeight; }
            set
            {
                imageHeight = value;
                OnPropertyChanged();
                ImageBound = new Rectangle(0, 0, Rate * ImageWidth, Rate * ImageHeight);
            }
        }

        private double xPos;
        public double XPos
        {
            get { return xPos; }
            set
            {
                xPos = value;
                OnPropertyChanged();
            }
        }
        private double yPos;
        public double YPos
        {
            get { return yPos; }
            set
            {
                yPos = value;
                OnPropertyChanged();
            }
        }
        private double radius;
        public double Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                OnPropertyChanged();
            }
        }
        private int repert;
        public int Repert
        {
            get { return repert; }
            set
            {
                repert = value;
                OnPropertyChanged();
            }
        }
        private int resolution;
        public int Resolution
        {
            get { return resolution; }
            set
            {
                resolution = value;
                OnPropertyChanged();
            }
        }
        private int juliaMapSplit;
        public int JuliaMapSplit
        {
            get { return juliaMapSplit; }
            set
            {
                juliaMapSplit = value;
                OnPropertyChanged();
            }
        }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged();
            }
        }

        public void ColorResolutionChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public MJViewModel()
        {
            UndoList = new UndoList<MJI>();
            XPos = 0;
            YPos = 0;
            Radius = 2;
            Repert = 63;
            Resolution = 4096;
            JuliaMapSplit = 11;
            Rate = 1;
            ImageHeight = 1024;
            ImageWidth = 1024;

            ColorParette = new ColorResolutionStruct[] {
                            new ColorResolutionStruct{ Color = Color.Black, Position = 0 },
                            new ColorResolutionStruct{ Color = Color.Red, Position = 0.15 },
                            new ColorResolutionStruct{ Color = Color.Orange, Position = 0.3 },
                            new ColorResolutionStruct{ Color = Color.Yellow, Position = 0.6 },
                            new ColorResolutionStruct{ Color = Color.White, Position = 1 }
                        };

            Undo = new Command(() => {
                MJI mji = UndoList.Undo();
                ImageSource = ImageSource.FromStream(() => new MemoryStream(mji.Bmp));
                FractalType = Double.IsNaN(mji.MJ.IPos) ? "MandelbrotSet" : "JuliaSet";
                XPos = Double.IsNaN(mji.MJ.IPos) ? mji.MJ.XPos : mji.MJ.IPos;
                YPos = Double.IsNaN(mji.MJ.JPos) ? mji.MJ.YPos : mji.MJ.JPos;
                Radius = mji.MJ.Radius;
                Repert = mji.MJ.Repert;
                Resolution = mji.MJ.Resolution;
                JuliaMapSplit = mji.MJ.JuliaMapSplit;
                UseColorParette = mji.ColorResolution != null;
                ColorParette = mji.ColorResolution?.ColorResolutions;
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }, () => UndoList.CanUndo);
            Redo = new Command(() => {
                MJI mji = UndoList.Redo();
                ImageSource = ImageSource.FromStream(() => new MemoryStream(mji.Bmp));
                FractalType = Double.IsNaN(mji.MJ.IPos) ? "MandelbrotSet" : "JuliaSet";
                XPos = Double.IsNaN(mji.MJ.IPos) ? mji.MJ.XPos : mji.MJ.IPos;
                YPos = Double.IsNaN(mji.MJ.JPos) ? mji.MJ.YPos : mji.MJ.JPos;
                Radius = mji.MJ.Radius;
                Repert = mji.MJ.Repert;
                Resolution = mji.MJ.Resolution;
                JuliaMapSplit = mji.MJ.JuliaMapSplit;
                UseColorParette = mji.ColorResolution != null;
                ColorParette = mji.ColorResolution?.ColorResolutions;
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }, () => UndoList.CanRedo);

            MandelbrotRun = new Command(async () => {
                MJI mji = new MJI {
                    MJ = new Mandelbrot_Julia(XPos, YPos, Radius, Repert, Resolution, ColorParette),
                    ColorResolution = (!UseColorParette ? null : new ColorResolution(Repert, ColorParette))
                };
                if (!UndoList.HasLast || UndoList.Last.MJ != mji.MJ)
                {
                    mji.MJ.Data = await Mandelbrot_Julia.Mandelbrot(mji.MJ.XPos, mji.MJ.YPos, mji.MJ.Radius, mji.MJ.Repert, mji.MJ.Resolution);
                    mji.MJ.Image = await Mandelbrot_Julia.Develop(mji.MJ.Repert, mji.MJ.Data, mji.ColorResolution);
                    mji.Bmp = await BitmapCreator.Create((short)mji.MJ.Resolution, (short)mji.MJ.Resolution, mji.MJ.Image, false);
                    UndoList.Push(mji);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else if (UndoList.Last.ColorResolution != mji.ColorResolution)
                {
                    mji.MJ.Data = UndoList.Last.MJ.Data;
                    mji.MJ.Image = await Mandelbrot_Julia.Develop(mji.MJ.Repert, mji.MJ.Data, mji.ColorResolution);
                    mji.Bmp = await BitmapCreator.Create((short)mji.MJ.Resolution, (short)mji.MJ.Resolution, mji.MJ.Image, false);
                    UndoList.Push(mji);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else
                {
                    mji.Bmp = UndoList.Last.Bmp;
                }
                ImageSource = ImageSource.FromStream(() => new MemoryStream(mji.Bmp));
                FractalType = "MandelbrotSet";
            });
            JuliaRun = new Command(async () => {
                MJI mji = new MJI {
                    MJ = new Mandelbrot_Julia(XPos, YPos, 0, 0, 2, Repert, Resolution, ColorParette),
                    ColorResolution = !UseColorParette ? null : new ColorResolution(Repert, ColorParette)
                };
                if (!UndoList.HasLast || UndoList.Last.MJ != mji.MJ)
                {
                    mji.MJ.Data = await Mandelbrot_Julia.Julia(mji.MJ.IPos, mji.MJ.JPos, mji.MJ.XPos, mji.MJ.YPos, mji.MJ.Radius, mji.MJ.Repert, mji.MJ.Resolution);
                    mji.MJ.Image = await Mandelbrot_Julia.Develop(mji.MJ.Repert, mji.MJ.Data, mji.ColorResolution);
                    mji.Bmp = await BitmapCreator.Create((short)mji.MJ.Resolution, (short)mji.MJ.Resolution, mji.MJ.Image, false);
                    UndoList.Push(mji);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else if (UndoList.Last.ColorResolution != mji.ColorResolution)
                {
                    mji.MJ.Data = UndoList.Last.MJ.Data;
                    mji.MJ.Image = await Mandelbrot_Julia.Develop(mji.MJ.Repert, mji.MJ.Data, mji.ColorResolution);
                    mji.Bmp = await BitmapCreator.Create((short)mji.MJ.Resolution, (short)mji.MJ.Resolution, mji.MJ.Image, false);
                    UndoList.Push(mji);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else
                {
                    mji.Bmp = UndoList.Last.Bmp;
                }
                ImageSource = ImageSource.FromStream(() => new MemoryStream(mji.Bmp));
                FractalType = "JuliaSet";
            });
            JuliaMupRun = new Command(async () => {
                int split = JuliaMapSplit == 0 ? 11 : JuliaMapSplit;
                Resolution = Resolution / split * split;
                MJI mji = new MJI {
                    MJ = new Mandelbrot_Julia(XPos, YPos, 2, Repert, Resolution, split, ColorParette),
                    ColorResolution = !UseColorParette ? null : new ColorResolution(Repert, ColorParette)
                };
                if (!UndoList.HasLast || UndoList.Last.MJ != mji.MJ)
                {
                    mji.MJ.Data = await Mandelbrot_Julia.JuliaMap(mji.MJ.XPos, mji.MJ.YPos, mji.MJ.Radius, mji.MJ.Repert, mji.MJ.Resolution, mji.MJ.JuliaMapSplit);
                    mji.MJ.Image = await Mandelbrot_Julia.Develop(mji.MJ.Repert, mji.MJ.Data, mji.ColorResolution);
                    mji.Bmp = await BitmapCreator.Create((short)mji.MJ.Resolution, (short)mji.MJ.Resolution, mji.MJ.Image, false);
                    UndoList.Push(mji);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else if (UndoList.Last.ColorResolution != mji.ColorResolution)
                {
                    mji.MJ.Data = UndoList.Last.MJ.Data;
                    mji.MJ.Image = await Mandelbrot_Julia.Develop(mji.MJ.Repert, mji.MJ.Data, mji.ColorResolution);
                    mji.Bmp = await BitmapCreator.Create((short)mji.MJ.Resolution, (short)mji.MJ.Resolution, mji.MJ.Image, false);
                    UndoList.Push(mji);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else
                {
                    mji.Bmp = UndoList.Last.Bmp;
                }
                ImageSource = ImageSource.FromStream(() => new MemoryStream(mji.Bmp));
                FractalType = "JuliaSet";
            });
        }
    }
}
