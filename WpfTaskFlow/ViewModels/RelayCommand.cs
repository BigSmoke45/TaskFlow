using System;
using System.Windows.Input;

namespace WpfTaskFlow.ViewModels
{
    // Універсальна реалізація ICommand – дозволяє передавати логіку через лямбди
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // Спрацьовує, коли WPF перевіряє, чи можна натиснути кнопку
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);

        // Говоримо WPF перевіряти ще раз CanExecute
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}