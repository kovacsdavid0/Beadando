using System.Windows.Input;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class OverviewViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage = new DashboardViewModel();

        public OverviewViewModel()
        {
            ShowDashboardCommand = new RelayCommand(_ => CurrentPage = new DashboardViewModel());
            ShowRestaurantsCommand = new RelayCommand(_ => CurrentPage = new RestaurantsViewModel());
            ShowMyReviewsCommand = new RelayCommand(_ => CurrentPage = new MyReviewsViewModel());
            ShowSettingsCommand = new RelayCommand(_ => CurrentPage = new SettingsViewModel());
            LogoutCommand = new RelayCommand(_ => LogoutRequested?.Invoke());
        }

        public event Action? LogoutRequested;

        public ICommand ShowDashboardCommand { get; }
        public ICommand ShowRestaurantsCommand { get; }
        public ICommand ShowMyReviewsCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand LogoutCommand { get; }

        public string Username => Session.User?.Username ?? string.Empty;

        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            private set => SetProperty(ref _currentPage, value);
        }
    }
}
