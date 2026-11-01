using System.Collections.ObjectModel;
using EatAndRate.Shared.DTOs.Franchises;
using EatAndRate.Shared.DTOs.Restaurants;
using EatAndRate.Shared.DTOs.Reviews;
using Frontend.Services;

namespace Frontend.ViewModels

{
    public class DashboardViewModel : ViewModelBase
    {
        private int _reviewsThisWeek;
        private string _favoriteFranchise = "–";
        private int _ratedFranchiseCount;
        private string _errorMessage = string.Empty;
        private bool _hasNoReviews;
        private FranchiseDto? _topFranchise;

        public DashboardViewModel()
        {
            // A konstruktor nem tud await-elni, a hibákat a LoadAsync kezeli
            _ = LoadAsync();
        }

        public string Username => Session.User?.Username ?? string.Empty;

        public int ReviewsThisWeek
        {
            get => _reviewsThisWeek;
            private set => SetProperty(ref _reviewsThisWeek, value);
        }

        public string FavoriteFranchise
        {
            get => _favoriteFranchise;
            private set => SetProperty(ref _favoriteFranchise, value);
        }

        public int RatedFranchiseCount
        {
            get => _ratedFranchiseCount;
            private set => SetProperty(ref _ratedFranchiseCount, value);
        }

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

        public ObservableCollection<RecentReviewItem> RecentReviews { get; } = new();

        public bool HasNoReviews
        {
            get => _hasNoReviews;
            private set => SetProperty(ref _hasNoReviews, value);
        }

        public FranchiseDto? TopFranchise
        {
            get => _topFranchise;
            private set
            {
                if (SetProperty(ref _topFranchise, value))
                {
                    OnPropertyChanged(nameof(HasTopFranchise));
                    OnPropertyChanged(nameof(TopFranchiseDetails));
                }
            }
        }

        public bool HasTopFranchise => TopFranchise is not null;

        public string TopFranchiseDetails => TopFranchise is null
            ? string.Empty
            : $"{TopFranchise.RestaurantCount} étterem · {TopFranchise.ReviewCount} értékelés";

        public ObservableCollection<RestaurantSummaryDto> TopRestaurants { get; } = new();

        private async Task LoadAsync()
        {
            try
            {
                var reviews = await ApiClient.GetMyReviewsAsync();
                ShowStats(reviews);
                ShowRecentReviews(reviews);
                await LoadTopFranchiseAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void ShowStats(List<ReviewDto> reviews)
        {
            // A CreatedAt UTC-ben van tárolva, a hét hétfőn kezdődik
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
            ReviewsThisWeek = reviews.Count(r => r.CreatedAt >= weekStart);

            var byFranchise = reviews
                .Where(r => r.FranchiseName is not null)
                .GroupBy(r => r.FranchiseName!)
                .ToList();

            RatedFranchiseCount = byFranchise.Count;

            // Kedvenc: a legtöbbet értékelt lánc, egyenlőségnél a jobb átlag dönt
            FavoriteFranchise = byFranchise
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Average(r => r.Overall))
                .Select(g => g.Key)
                .FirstOrDefault() ?? "Még nincs";
        }

        private void ShowRecentReviews(List<ReviewDto> reviews)
        {
            // Az API a legfrissebbel kezdve adja vissza a listát
            foreach (var review in reviews.Take(4))
                RecentReviews.Add(new RecentReviewItem(review));

            HasNoReviews = reviews.Count == 0;
        }

        private async Task LoadTopFranchiseAsync()
        {
            var franchises = await ApiClient.GetFranchisesAsync();

            TopFranchise = franchises
                .Where(f => f.AvgOverall is not null)
                .OrderByDescending(f => f.AvgOverall)
                .FirstOrDefault();

            if (TopFranchise is null)
                return;

            var restaurants = await ApiClient.GetRestaurantsByFranchiseAsync(TopFranchise.Id);

            foreach (var restaurant in restaurants
                .Where(r => r.ReviewCount > 0)
                .OrderByDescending(r => r.AvgOverall)
                .Take(3))
            {
                TopRestaurants.Add(restaurant);
            }
        }
    }
}