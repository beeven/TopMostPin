using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TopMostPin
{
    public class Command : ICommand
    {
        public Action<object> CommandAction { get; set; }
        public Func<object, bool> CanExecuteFunc { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            CommandAction?.Invoke(parameter);
        }
    }
}
