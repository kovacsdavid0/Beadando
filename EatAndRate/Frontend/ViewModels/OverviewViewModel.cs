using System.Windows.Input;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class OverviewViewModel : ViewModelBase
    {
        public OverviewViewModel()
        {
            LogoutCommand = new RelayCommand(_ => LogoutRequested?.Invoke());
        }

        public event Action? LogoutRequested;

        public ICommand LogoutCommand { get; }

        public string Username => Session.User?.Username ?? string.Empty;
    }
}
