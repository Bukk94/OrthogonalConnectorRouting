using System;
using System.Windows.Input;

namespace SampleApp
{
    public class RelayParamCommand : ICommand
    {
        private Action<object> _action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayParamCommand(Action<object> action)
        {
            this._action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this._action(parameter);
        }
    }
}
