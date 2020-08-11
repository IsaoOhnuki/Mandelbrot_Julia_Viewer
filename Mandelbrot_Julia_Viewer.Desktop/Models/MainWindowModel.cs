using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mandelbrot_Julia_Viewer.Desktop.Models
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
        public void SetProperty<T>(ref T storage, T value, string propertyName)
        {
            if (!storage.Equals(value))
            {
                storage = value;
                OnPropertyChanged(propertyName);
            }
        }
    }

    public class MainWindowModel : BindableBase
    {
        MJ _mjParam = new MJ();

        static MainWindowModel _instance;
        public static MainWindowModel Instance => _instance ?? (_instance = new MainWindowModel());

        public MainWindowModel()
        {
        }

        public double XPos { get { return _mjParam.XPos; } set { double XPos = 0.0; SetProperty(ref XPos, value, nameof(XPos)); _mjParam.XPos = XPos; } }
        public double YPos { get { return _mjParam.YPos; } set { double YPos = 0.0; SetProperty(ref YPos, value, nameof(YPos)); _mjParam.YPos = YPos; } }
        public double IPos { get { return _mjParam.IPos; } set { double IPos = 0.0; SetProperty(ref IPos, value, nameof(IPos)); _mjParam.IPos = IPos; } }
        public double JPos { get { return _mjParam.JPos; } set { double JPos = 0.0; SetProperty(ref JPos, value, nameof(JPos)); _mjParam.JPos = JPos; } }
        public double Radius { get { return _mjParam.Radius; } set { double Radius = 0.0; SetProperty(ref Radius, value, nameof(Radius)); _mjParam.Radius = Radius; } }
        public short Repert { get { return _mjParam.Repert; } set { short Repert = 0; SetProperty(ref Repert, value, nameof(Repert)); _mjParam.Repert = Repert; } }
        public int Resolution { get { return _mjParam.Resolution; } set { int Resolution = 0; SetProperty(ref Resolution, value, nameof(Resolution)); _mjParam.Resolution = Resolution; } }
    }
}
