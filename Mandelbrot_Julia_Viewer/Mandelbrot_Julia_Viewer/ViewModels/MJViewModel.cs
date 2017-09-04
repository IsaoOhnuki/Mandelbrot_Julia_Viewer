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
    // http://garicchi.com/?p=16481 プリンターを使って印刷するには
    public class MJViewModel : BindableObject
    {
        public Command MandelbrotRun { get; set; }
        public Command JuliaRun { get; set; }
        public Command Undo { get; set; }
        public Command Redo { get; set; }
        public bool CanUndo { get { return UndoList.CanUndo; } }
        public bool CanRedo { get { return UndoList.CanRedo; } }
        public UndoList<Mandelbrot_Julia> UndoList { get; set; }
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
        private Mandelbrot_Julia.ColorResolutionStruct[] colorParette;
        public Mandelbrot_Julia.ColorResolutionStruct[] ColorParette
        {
            get { return colorParette; }
            set
            {
                //colorParette = value;
                colorParette = null;
                if (value != null)
                {
                    colorParette = new Mandelbrot_Julia.ColorResolutionStruct[value.GetLength(0)];
                    value.CopyTo(colorParette, 0);
                }
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
            UndoList = new UndoList<Mandelbrot_Julia>();
            XPos = 0;
            YPos = 0;
            Radius = 2;
            Repert = 63;
            Resolution = 1024;
            Rate = 1;
            ImageHeight = 1024;
            ImageWidth = 1024;

            ColorParette = new Mandelbrot_Julia.ColorResolutionStruct[] {
                            new Mandelbrot_Julia.ColorResolutionStruct{ Color = Color.Black, Posision = 0 },
                            new Mandelbrot_Julia.ColorResolutionStruct{ Color = Color.Red, Posision = 0.15 },
                            new Mandelbrot_Julia.ColorResolutionStruct{ Color = Color.DarkViolet, Posision = 0.3 },
                            new Mandelbrot_Julia.ColorResolutionStruct{ Color = Color.Blue, Posision = 0.6 },
                            new Mandelbrot_Julia.ColorResolutionStruct{ Color = Color.White, Posision = 1 }
                        };

            Undo = new Command(async () => {
                Mandelbrot_Julia mj = UndoList.Undo();
                byte[] bmp = await BitmapCreator.Create((short)mj.Resolution, (short)mj.Resolution, mj.Image);
                ImageSource = ImageSource.FromStream(() => new MemoryStream(bmp));
                FractalType = Double.IsNaN(mj.IPos) ? "MandelbrotSet" : "JuliaSet";
                XPos = Double.IsNaN(mj.IPos) ? mj.XPos : mj.IPos;
                YPos = Double.IsNaN(mj.JPos) ? mj.YPos : mj.JPos;
                Radius = mj.Radius;
                Repert = mj.Repert;
                Resolution = mj.Resolution;
                UseColorParette = mj.ParetteType > 0;
                ColorParette = mj.ColorParette.ColorResolutions;
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }, () => UndoList.CanUndo);
            Redo = new Command(async () => {
                Mandelbrot_Julia mj = UndoList.Redo();
                byte[] bmp = await BitmapCreator.Create((short)mj.Resolution, (short)mj.Resolution, mj.Image);
                ImageSource = ImageSource.FromStream(() => new MemoryStream(bmp));
                FractalType = Double.IsNaN(mj.IPos) ? "MandelbrotSet" : "JuliaSet";
                XPos = Double.IsNaN(mj.IPos) ? mj.XPos : mj.IPos;
                YPos = Double.IsNaN(mj.JPos) ? mj.YPos : mj.JPos;
                Radius = mj.Radius;
                Repert = mj.Repert;
                Resolution = mj.Resolution;
                UseColorParette = mj.ParetteType > 0;
                ColorParette = mj.ColorParette.ColorResolutions;
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }, () => UndoList.CanRedo);

            MandelbrotRun = new Command(async () => {
                Mandelbrot_Julia mj = new Mandelbrot_Julia(XPos, YPos, Radius, Repert, Resolution, UseColorParette ? 1 : 0, ColorParette);
                byte[] bmp;
                if (!UndoList.HasLast || UndoList.Last != mj)
                {
                    mj.Data = await Mandelbrot_Julia.Mandelbrot(mj.XPos, mj.YPos, mj.Radius, mj.Repert, mj.Resolution);
                    mj.Image = await Mandelbrot_Julia.Develop(mj.Repert, mj.Data, mj.ParetteType > 0 ? mj.ColorParette : null);
                    bmp = await BitmapCreator.Create((short)mj.Resolution, (short)mj.Resolution, mj.Image);
                    UndoList.Push(mj);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else
                {
                    bmp = await BitmapCreator.Create((short)UndoList.Last.Resolution, (short)UndoList.Last.Resolution, UndoList.Last.Image);
                }
                ImageSource = ImageSource.FromStream(() => new MemoryStream(bmp));
                FractalType = "MandelbrotSet";
            });
            JuliaRun = new Command(async () => {
                Mandelbrot_Julia mj = new Mandelbrot_Julia(XPos, YPos, 0, 0, 2, Repert, Resolution, UseColorParette ? 1 : 0, ColorParette);
                byte[] bmp;
                if (!UndoList.HasLast || UndoList.Last != mj)
                {
                    mj.Data = await Mandelbrot_Julia.Julia(mj.IPos, mj.JPos, mj.XPos, mj.YPos, mj.Radius, mj.Repert, mj.Resolution);
                    mj.Image = await Mandelbrot_Julia.Develop(mj.Repert, mj.Data, mj.ParetteType > 0 ? mj.ColorParette: null);
                    bmp = await BitmapCreator.Create((short)mj.Resolution, (short)mj.Resolution, mj.Image);
                    UndoList.Push(mj);
                    OnPropertyChanged(nameof(CanUndo));
                    OnPropertyChanged(nameof(CanRedo));
                }
                else
                {
                    bmp = await BitmapCreator.Create((short)UndoList.Last.Resolution, (short)UndoList.Last.Resolution, UndoList.Last.Image);
                }
                ImageSource = ImageSource.FromStream(() => new MemoryStream(bmp));
                FractalType = "JuliaSet";
            });
        }
    }
}
