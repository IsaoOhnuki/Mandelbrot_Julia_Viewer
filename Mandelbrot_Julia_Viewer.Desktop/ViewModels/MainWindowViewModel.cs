using Mandelbrot_Julia_Viewer.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mandelbrot_Julia_Viewer.Desktop.ViewModels
{
    public class CommandHandler : ICommand
    {
        private readonly Func<object, bool> _CanExecute;
        private readonly Action<object> _Execute;

        public CommandHandler(Action<object> Execute, Func<object, bool> CanExecute = null)
        {
            _Execute = Execute;
            _CanExecute = CanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_CanExecute == null)
            {
                return true;
            }
            return _CanExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            _Execute?.Invoke(parameter);
        }

        public void DoCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class MainWindowViewModel : DependencyObject
    {
        public MainWindowViewModel()
        {
            MainWindowModel.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(XPos)) XPos = MainWindowModel.Instance.XPos;
            else if (e.PropertyName == nameof(YPos)) YPos = MainWindowModel.Instance.YPos;
            else if (e.PropertyName == nameof(JPos)) JPos = MainWindowModel.Instance.JPos;
            else if (e.PropertyName == nameof(IPos)) IPos = MainWindowModel.Instance.IPos;
            else if (e.PropertyName == nameof(Radius)) Radius = MainWindowModel.Instance.Radius;
            else if (e.PropertyName == nameof(Repert)) Repert = MainWindowModel.Instance.Repert;
            else if (e.PropertyName == nameof(Resolution)) Resolution = MainWindowModel.Instance.Resolution;
        }

        public double XPos
        {
            get { return (double)GetValue(XPosProperty); }
            set { SetValue(XPosProperty, value); }
        }

        public static readonly DependencyProperty XPosProperty =
            DependencyProperty.Register(
                nameof(XPos),
                typeof(double),
                typeof(MainWindowViewModel),
                new PropertyMetadata());

        public double YPos
        {
            get { return (double)GetValue(YPosProperty); }
            set { SetValue(YPosProperty, value); }
        }

        public static readonly DependencyProperty YPosProperty =
            DependencyProperty.Register(
                nameof(YPos),
                typeof(double),
                typeof(MainWindowViewModel),
                new PropertyMetadata());

        public double IPos
        {
            get { return (double)GetValue(IPosProperty); }
            set { SetValue(IPosProperty, value); }
        }

        public static readonly DependencyProperty IPosProperty =
            DependencyProperty.Register(
                nameof(IPos),
                typeof(double),
                typeof(MainWindowViewModel),
                new PropertyMetadata());

        public double JPos
        {
            get { return (double)GetValue(JPosProperty); }
            set { SetValue(JPosProperty, value); }
        }

        public static readonly DependencyProperty JPosProperty =
            DependencyProperty.Register(
                nameof(JPos),
                typeof(double),
                typeof(MainWindowViewModel),
                new PropertyMetadata());

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(
                nameof(Radius),
                typeof(double),
                typeof(MainWindowViewModel),
                new PropertyMetadata());

        public short Repert
        {
            get { return (short)GetValue(RepertProperty); }
            set { SetValue(RepertProperty, value); }
        }

        public static readonly DependencyProperty RepertProperty =
            DependencyProperty.Register(
                nameof(Repert),
                typeof(double),
                typeof(MainWindowViewModel),
                new PropertyMetadata());

        public int Resolution
        {
            get { return (int)GetValue(ResolutionProperty); }
            set { SetValue(ResolutionProperty, value); }
        }

        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register(
                nameof(Resolution),
                typeof(int),
                typeof(MainWindowViewModel),
                new PropertyMetadata());

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(
                nameof(Image),
                typeof(ImageSource),
                typeof(MainWindowViewModel),
                new PropertyMetadata());
    }
}
