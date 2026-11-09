using EatAndRate.Shared.DTOs.Restaurants;
using EatAndRate.Shared.DTOs.Reviews;
using Frontend.Services;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;


namespace Frontend.ViewModels
{
    public class CreateReviewViewModel : ViewModelBase
    {
        private readonly int _restaurantId;
        private readonly ReviewDto? _review;
        private int _foodRating, _serviceRating, _atmosphereRating;
        private string? _comment;
        private bool _isBusy;
        private string _errorMessage = string.Empty;

        // Új értékelés
        public CreateReviewViewModel(RestaurantSummaryDto restaurant)
            : this(restaurant.Id, restaurant.Name, null)
        {
        }

        // Meglévő értékelés módosítása
        public CreateReviewViewModel(ReviewDto review)
            : this(review.RestaurantId, review.RestaurantName, review)
        {
            FoodRating = review.FoodRating;
            ServiceRating = review.ServiceRating;
            AtmosphereRating = review.AtmosphereRating;
            Comment = review.Comment;
        }

        private CreateReviewViewModel(int restaurantId, string restaurantName, ReviewDto? review)
        {
            _restaurantId = restaurantId;
            _review = review;
            RestaurantName = restaurantName;
            SaveCommand = new AsyncRelayCommand(_ => SaveAsync(), _ => !IsBusy, ex => ErrorMessage = $"Váratlan hiba történt: {ex.Message}");
            DeleteCommand = new AsyncRelayCommand(_ => DeleteAsync(), _ => !IsBusy, ex => ErrorMessage = $"Váratlan hiba történt: {ex.Message}");
            CancelCommand = new RelayCommand(_ => CloseRequested?.Invoke(), _ => !IsBusy);
        }

        public event Action? CloseRequested;
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public string RestaurantName { get; }

        public bool IsSaved { get; private set; }

        public bool IsEditMode => _review is not null;
        public string FormTitle => IsEditMode ? "Értékelés módosítása" : "Étterem értékelése";

        public double Overall => (FoodRating + ServiceRating + AtmosphereRating) / 3.0;
        public string OverallLabel => Overall switch
        {
            >= 4.5 => "Kiváló élmény",
            >= 3.5 => "Jó élmény",
            >= 2.5 => "Átlagos élmény",
            >= 1.5 => "Gyenge élmény",
            _ => "Rossz élmény"
        };

        public List<StarItem> FoodStars => CreateStars(FoodRating, v => FoodRating = v);
        public List<StarItem> ServiceStars => CreateStars(ServiceRating, v => ServiceRating = v);
        public List<StarItem> AtmosphereStars => CreateStars(AtmosphereRating, v => AtmosphereRating = v);
        public List<StarItem> OverallStars => CreateStars((int)Math.Round(Overall, MidpointRounding.AwayFromZero), null);

        public int FoodRating
        {
            get => _foodRating;
            set { if (SetProperty(ref _foodRating, value)) OnRatingChanged(nameof(FoodStars)); }
        }
        public int ServiceRating
        {
            get => _serviceRating;
            set { if (SetProperty(ref _serviceRating, value)) OnRatingChanged(nameof(ServiceStars)); }
        }
        public int AtmosphereRating
        {
            get => _atmosphereRating;
            set { if (SetProperty(ref _atmosphereRating, value)) OnRatingChanged(nameof(AtmosphereStars)); }
        }

        public string? Comment
        {
            get => _comment;
            set
            {
                if (SetProperty(ref _comment, value))
                {
                    OnPropertyChanged(nameof(CommentLength));
                    OnPropertyChanged(nameof(IsCommentPlaceholderVisible));
                }
            }
        }
        public int CommentLength => Comment?.Length ?? 0;
        public bool IsCommentPlaceholderVisible => string.IsNullOrEmpty(Comment);

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

        private void OnRatingChanged(string starsProperty)
        {
            OnPropertyChanged(starsProperty);
            OnPropertyChanged(nameof(Overall));
            OnPropertyChanged(nameof(OverallStars));
            OnPropertyChanged(nameof(OverallLabel));
        }

        private static List<StarItem> CreateStars(int rating, Action<int>? select) =>
            Enumerable.Range(1, 5)
                .Select(v => new StarItem(v <= rating, select is null ? null : new RelayCommand(_ => select(v))))
                .ToList();

        private async Task SaveAsync()
        {
            if (FoodRating == 0 || ServiceRating == 0 || AtmosphereRating == 0)
            {
                ErrorMessage = "Kérlek, mindhárom szempontot pontozd 1-től 5-ig.";
                return;
            }
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
                if (_review is null)
                    await ApiClient.CreateReviewAsync(_restaurantId, dto);
                else
                    await ApiClient.UpdateReviewAsync(_review.Id, dto);

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

        private async Task DeleteAsync()
        {
            if (_review is null)
                return;

            var answer = MessageBox.Show("Biztosan törlöd ezt az értékelést?", "Értékelés törlése",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (answer != MessageBoxResult.Yes)
                return;

            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                await ApiClient.DeleteReviewAsync(_review.Id);
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

    public record StarItem(bool IsFilled, ICommand? SelectCommand);
}