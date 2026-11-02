using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;
using EatAndRate.Shared.DTOs.Reviews;
using Frontend.Services;

namespace Frontend.ViewModels
{
    public class ReviewDetailViewModel : ViewModelBase
    {
        private readonly ReviewDto _review;
        private int _foodRating;
        private int _serviceRating;
        private int _atmosphereRating;
        private string? _comment;
        private bool _isEditing;
        private bool _isBusy;
        private string _errorMessage = string.Empty;

        public ReviewDetailViewModel(ReviewDto review)
        {
            _review = review;
            ResetFields();

            EditCommand = new RelayCommand(_ => IsEditing = true, _ => !IsBusy);
            CancelCommand = new RelayCommand(_ => CancelEdit(), _ => !IsBusy);
            SaveCommand = new AsyncRelayCommand(_ => SaveAsync(), _ => !IsBusy,
                ex => ErrorMessage = $"Váratlan hiba történt: {ex.Message}");
            DeleteCommand = new AsyncRelayCommand(_ => DeleteAsync(), _ => !IsBusy,
                ex => ErrorMessage = $"Váratlan hiba történt: {ex.Message}");
        }

        public event Action? CloseRequested;

        public ICommand EditCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public int[] RatingOptions { get; } = { 1, 2, 3, 4, 5 };

        public string RestaurantName => _review.RestaurantName;
        public string? FranchiseName => _review.FranchiseName;
        public string? City => _review.City;
        public DateTime CreatedAt => _review.CreatedAt;

        // A lista ebből tudja, hogy újra kell-e töltenie magát
        public bool HasChanges { get; private set; }

        public double Overall => (FoodRating + ServiceRating + AtmosphereRating) / 3.0;

        public int FoodRating
        {
            get => _foodRating;
            set { if (SetProperty(ref _foodRating, value)) OnPropertyChanged(nameof(Overall)); }
        }

        public int ServiceRating
        {
            get => _serviceRating;
            set { if (SetProperty(ref _serviceRating, value)) OnPropertyChanged(nameof(Overall)); }
        }

        public int AtmosphereRating
        {
            get => _atmosphereRating;
            set { if (SetProperty(ref _atmosphereRating, value)) OnPropertyChanged(nameof(Overall)); }
        }

        public string? Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            private set
            {
                if (SetProperty(ref _isEditing, value))
                    OnPropertyChanged(nameof(IsNotEditing));
            }
        }

        public bool IsNotEditing => !IsEditing;

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (SetProperty(ref _errorMessage, value))
                    OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                    RelayCommand.RaiseCanExecuteChanged();
            }
        }

        private void ResetFields()
        {
            FoodRating = _review.FoodRating;
            ServiceRating = _review.ServiceRating;
            AtmosphereRating = _review.AtmosphereRating;
            Comment = _review.Comment;
        }

        private void CancelEdit()
        {
            ResetFields();
            ErrorMessage = string.Empty;
            IsEditing = false;
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
                await ApiClient.UpdateReviewAsync(_review.Id, dto);

                _review.FoodRating = dto.FoodRating;
                _review.ServiceRating = dto.ServiceRating;
                _review.AtmosphereRating = dto.AtmosphereRating;
                _review.Comment = dto.Comment;
                Comment = dto.Comment;

                HasChanges = true;
                IsEditing = false;
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
            var answer = MessageBox.Show("Biztosan törlöd ezt az értékelést?", "Értékelés törlése",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (answer != MessageBoxResult.Yes)
                return;

            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                await ApiClient.DeleteReviewAsync(_review.Id);
                HasChanges = true;
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