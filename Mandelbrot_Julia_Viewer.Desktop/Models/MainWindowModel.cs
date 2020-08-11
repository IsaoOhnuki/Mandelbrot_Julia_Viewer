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
            storage = value;
            OnPropertyChanged(propertyName);
        }
    }

    public class MainWindowModel : BindableBase
    {
        MainWindowModel _instance;
        public MainWindowModel Instance => _instance ?? (_instance = new MainWindowModel());

        public MainWindowModel()
        {
        }
    }
}
