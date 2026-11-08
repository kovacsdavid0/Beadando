using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using EatAndRate.Shared.DTOs.Restaurants;
using EatAndRate.Shared.DTOs.Reviews;
using Frontend.Services;


namespace Frontend.ViewModels
{
    public class CreateReviewViewModel : ViewModelBase
    {
        private readonly RestaurantSummaryDto _restaurant;
        private int _foodRating = 5, _serviceRating = 5, _atmosphereRating = 5;
        private string? _comment;
        private bool _isBusy;
        private string _errorMessage = string.Empty;

        public CreateReviewViewModel(RestaurantSummaryDto restaurant)
        {
            _restaurant = restaurant;
            SaveCommand = new AsyncRelayCommand(_ => SaveAsync(), _ => !IsBusy, ex => ErrorMessage = $"Váratlan hiba történt: {ex.Message}");
        }

        public event Action? CloseRequested;
        public ICommand SaveCommand { get; }
        public int[] RatingOptions { get; } = { 1, 2, 3, 4, 5 };
        public string RestaurantName => _restaurant.Name;
        public bool IsSaved { get; private set; }

        public double Overall => (FoodRating + ServiceRating + AtmosphereRating) / 3.0;

        public int FoodRating
        {
            get => _foodRating;
            set { if (SetProperty(ref _foodRating, value))
                    OnPropertyChanged(nameof(Overall)); }
        }
        public int ServiceRating
        {
            get => _serviceRating;
            set { if (SetProperty(ref _serviceRating, value))
                    OnPropertyChanged(nameof(Overall)); }
        }
        public int AtmosphereRating
        {
            get => _atmosphereRating;
            set { if (SetProperty(ref _atmosphereRating, value))
                    OnPropertyChanged(nameof(Overall)); }
        }
        public string? Comment { get => _comment; set => SetProperty(ref _comment, value); }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set { if (SetProperty(ref _errorMessage, value)) OnPropertyChanged(nameof(HasError)); }
        }
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool IsBusy
        {
            get => _isBusy;
            private set { if (SetProperty(ref _isBusy, value)) RelayCommand.RaiseCanExecuteChanged(); }
        }

        private async Task SaveAsync()
        {
            var dto = new CreateReviewDto
            {
                FoodRating = FoodRating,
                ServiceRating = ServiceRating,
                AtmosphereRating = AtmosphereRating,
                Comment = string.IsNullOrWhiteSpace(Comment) ? null : Comment.Trim()
            };

            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, new ValidationContext(dto), results, true))
            {
                ErrorMessage = string.Join("\n", results.Select(r => r.ErrorMessage));
                return;
            }

            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                await ApiClient.CreateReviewAsync(_restaurant.Id, dto);
                IsSaved = true;
                CloseRequested?.Invoke();
            }
            catch (ApiException ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}