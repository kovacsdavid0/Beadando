using System.DirectoryServices;
using System.Globalization;
using System.Security.Policy;
using System.Windows;
using System.Windows.Input;
using EatAndRate.Shared.DTOs.Reviews;
using Frontend.Services;
using Frontend.Views;

namespace Frontend.ViewModels
{
    public record ReviewSortOption(string Label, Func<IEnumerable<ReviewDto>, IEnumerable<ReviewDto>> Sort);

    public class MyReviewsViewModel : ViewModelBase
    {
        private static readonly StringComparer HungarianOrder = StringComparer.Create(CultureInfo.GetCultureInfo("hu-HU"), ignoreCase: true);

        private List<ReviewDto> _allReviews = new();
        private IReadOnlyList<ReviewDto> _reviews = Array.Empty<ReviewDto>();
        private ReviewSortOption _selectedSort;
        private string _errorMessage = string.Empty;
        private bool _isLoaded;

        public MyReviewsViewModel()
        {
            _selectedSort = SortOptions[0];
            OpenReviewCommand = new AsyncRelayCommand(p => OpenReviewAsync((ReviewDto)p!));
            _ = LoadAsync();
        }

        public IReadOnlyList<ReviewSortOption> SortOptions { get; } = new ReviewSortOption[]
        {
            new("Dátum (legújabb elől)", r => r.OrderByDescending(x => x.CreatedAt)),
            new("Dátum (legrégebbi elől)", r => r.OrderBy(x => x.CreatedAt)),
            new("Étterem neve (A-Z)", r => r.OrderBy(x => x.RestaurantName, HungarianOrder)),
            new("Étterem neve (Z-A)", r => r.OrderByDescending(x => x.RestaurantName, HungarianOrder)),
            new("Franchise neve (A-Z)", r => r.OrderBy(x => x.FranchiseName is null).ThenBy(x => x.FranchiseName, HungarianOrder)),
            new("Franchise neve (Z-A)", r => r.OrderBy(x => x.FranchiseName is null).ThenByDescending(x => x.FranchiseName, HungarianOrder)),
            new("Értékelés (legjobb elől)", r => r.OrderByDescending(x => x.Overall)),
            new("Értékelés (legrosszabb elől)", r => r.OrderBy(x => x.Overall))

        };

        public ICommand OpenReviewCommand { get; }

        public IReadOnlyList<ReviewDto> Reviews
        {
            get => _reviews;
            private set => SetProperty(ref _reviews, value);
        }

        public ReviewSortOption SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (SetProperty(ref _selectedSort, value)) 
                    ApplySort();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool IsEmpty => _isLoaded && _allReviews.Count == 0;

        private void ApplySort() => Reviews = SelectedSort.Sort(_allReviews).ToList();

        private async Task LoadAsync()
        {
            try
            {
                _allReviews = await ApiClient.GetMyReviewsAsync();
                foreach (var review in _allReviews)
                    review.CreatedAt = review.CreatedAt.ToLocalTime();

                _isLoaded = true;
                ErrorMessage = string.Empty;
                OnPropertyChanged(nameof(IsEmpty));
                ApplySort();
            }
            catch (ApiException ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task OpenReviewAsync(ReviewDto review)
        {
            var detail = new ReviewDetailViewModel(review);
            var window = new ReviewDetailWindow(detail) { Owner = Application.Current.MainWindow };
            window.ShowDialog();

            if (detail.HasChanges)
                await LoadAsync();
        }
    }
}
