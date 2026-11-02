using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Windows.Data;
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
        private string _searchText = string.Empty;
        private string _franchiseSearchText = string.Empty;

        public RestaurantsViewModel()
        {
            OpenFranchiseCommand = new AsyncRelayCommand(p =>
                OpenFranchiseAsync((FranchiseDto)p!));
            BackCommand = new RelayCommand(_ =>
            {
                ErrorMessage = String.Empty;
                SelectedFranchise = null;
            });
            CollectionViewSource.GetDefaultView(Restaurants).Filter = MatchesSearch;
            CollectionViewSource.GetDefaultView(Franchises).Filter = MatchesFranchiseSearch;
            _ = LoadFranchisesAsync();
        }

        public ObservableCollection<FranchiseDto> Franchises { get; } = new();
        public ObservableCollection<RestaurantSummaryDto> Restaurants { get; } = new();

        public ICommand OpenFranchiseCommand { get; }
        public ICommand BackCommand { get; }

        public bool IsFranchiseListVisible => SelectedFranchise is null;
        public bool IsRestaurantListVisible => SelectedFranchise is not null;
        public bool IsSearchPlaceholderVisible => string.IsNullOrEmpty(SearchText);
        public bool IsFranchiseSearchPlaceholderVisible => string.IsNullOrEmpty(FranchiseSearchText);

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

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    OnPropertyChanged(nameof(IsSearchPlaceholderVisible));
                    CollectionViewSource.GetDefaultView(Restaurants).Refresh();
                }
            }
        }
        public string FranchiseSearchText
        {
            get => _franchiseSearchText;
            set
            {
                if (SetProperty(ref _franchiseSearchText, value))
                {
                    OnPropertyChanged(nameof(IsFranchiseSearchPlaceholderVisible));
                    CollectionViewSource.GetDefaultView(Franchises).Refresh();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        private bool MatchesSearch(object item)
        {
            var restaurant = (RestaurantSummaryDto)item;
            var search = SearchText.Trim();

            return search.Length == 0
                || restaurant.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                || restaurant.City?.Contains(search, StringComparison.CurrentCultureIgnoreCase) == true
                || restaurant.Address?.Contains(search, StringComparison.CurrentCultureIgnoreCase) == true;
        }
        private bool MatchesFranchiseSearch(object item)
        {
            var franchise = (FranchiseDto)item;
            var search = FranchiseSearchText.Trim();

            return search.Length == 0
                || franchise.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase);
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
            SearchText = string.Empty;
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