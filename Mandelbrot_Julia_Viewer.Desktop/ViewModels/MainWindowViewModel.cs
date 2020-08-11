using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
    }
}
