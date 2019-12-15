using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Well_Podcast.Common
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _executeAsync;
        private readonly Action<object> _executeParamAsync;
        private readonly Func<bool> _canExecuteAsync;

        public DelegateCommand()
        {
            this._executeAsync = delegate { };
            this._canExecuteAsync = () => true;
        }
        public DelegateCommand(Action execute)
        {
            this._executeAsync = execute;
            this._canExecuteAsync = () => true;
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this._executeAsync = execute;
            this._canExecuteAsync = canExecute;
        }

        public DelegateCommand(Action<object> executeParam, Func<bool> canExecute)
        {
            this._executeParamAsync = executeParam;
            this._canExecuteAsync = canExecute;
        }


        public event EventHandler CanExecuteChanged;
        public void RaiseExecuteChanged()
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
        }
        public bool CanExecute(object parameter)
        {
            return this._canExecuteAsync();
        }
        public void Execute(object parameter)
        {
            if (this._executeAsync != null)
                this._executeAsync();
            else if (this._executeParamAsync != null)
                this._executeParamAsync(parameter);
            else
                System.Diagnostics.Debug.WriteLine("Action not set");
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            try
            {
                _execute = execute;
            }
            catch { new ArgumentNullException(nameof(execute)); }

            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        public void Execute(object parameter) => _execute((T)parameter);

        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
