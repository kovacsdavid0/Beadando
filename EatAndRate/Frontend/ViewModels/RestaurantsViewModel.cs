using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Windows.Input;
using EatAndRate.Shared.DTOs.Franchises;
using EatAndRate.Shared.DTOs.Restaurants;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class RestaurantsViewModel : ViewModelBase
    {
        private FranchiseDto? _selectedFranchise;
        private string _errorMessage = string.Empty;

        public RestaurantsViewModel()
        {
            OpenFranchiseCommand = new AsyncRelayCommand(p =>
                OpenFranchiseAsync((FranchiseDto)p!));
            BackCommand = new RelayCommand(_ =>
            {
                ErrorMessage = String.Empty;
                SelectedFranchise = null;
            });
            _ = LoadFranchisesAsync();
        }

        public ObservableCollection<FranchiseDto> Franchises { get; } = new();
        public ObservableCollection<RestaurantSummaryDto> Restaurants { get; } = new();

        public ICommand OpenFranchiseCommand { get; }
        public ICommand BackCommand { get; }

        public bool IsFranchiseListVisible => SelectedFranchise is null;
        public bool IsRestaurantListVisible => SelectedFranchise is not null;

        public FranchiseDto? SelectedFranchise
        {
            get => _selectedFranchise;
            private set
            {
                if (SetProperty(ref _selectedFranchise, value))
                {
                    OnPropertyChanged(nameof(IsFranchiseListVisible));
                    OnPropertyChanged(nameof(IsRestaurantListVisible));
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }
        private async Task LoadFranchisesAsync()
        {
            try
            {
                foreach (var franchise in await ApiClient.GetFranchisesAsync())
                    Franchises.Add(franchise);
            }
            catch (ApiException ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        private async Task OpenFranchiseAsync(FranchiseDto franchise)
        {
            ErrorMessage = string.Empty;
            Restaurants.Clear();
            SelectedFranchise = franchise;
            try
            {
                foreach (var restaurant in await ApiClient.GetRestaurantsByFranchiseAsync(franchise.Id))
                    Restaurants.Add(restaurant);
            }
            catch (ApiException ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
