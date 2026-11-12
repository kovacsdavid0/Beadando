using System.Windows;
using System.Windows.Input;

namespace Frontend.ViewModels
{

    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Func<object?, bool>? _canExecute;
        private readonly Action<Exception>? _onException;

        public AsyncRelayCommand(Func<object?, Task> execute,
                                 Func<object?, bool>? canExecute = null,
                                 Action<Exception>? onException = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _onException = onException;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;


        public async void Execute(object? parameter)
        {
            try
            {
                await _execute(parameter);
            }
            catch (Exception ex)
            {
                if (_onException is not null)
                    _onException(ex);
                else
                    MessageBox.Show(ex.Message, "Váratlan hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
