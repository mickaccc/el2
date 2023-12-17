﻿using System;
using System.Windows.Input;

namespace El2Core.Utils
{
    public class ActionCommand : ICommand
    {
        private readonly Action<object> _executeHandler;
        private readonly Func<object, bool> _canExecuteHandler;
        private Action<object> onSaveExecuted;
        private object? onSaveCanExecute;

        public ActionCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _executeHandler = execute ?? throw new ArgumentNullException("Execute cannot be null");
            _canExecuteHandler = canExecute;
        }

        public ActionCommand(Action<object> onSaveExecuted, object? onSaveCanExecute)
        {
            this.onSaveExecuted = onSaveExecuted;
            this.onSaveCanExecute = onSaveCanExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter)
        {
            _executeHandler(parameter);
        }
        public bool CanExecute(object parameter)
        {
            if (_canExecuteHandler == null)
                return true;
            return _canExecuteHandler(parameter);
        }
    }
}
