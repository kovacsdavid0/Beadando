using System;
using System.Collections.Generic;
using System.Text;
using EatAndRate.Shared.DTOs.Reviews;

namespace Frontend.ViewModels
{
    // Egy sor a Legutóbbi értékelések listában, megjelenítésre előkészítve
    public class RecentReviewItem
    {
        public RecentReviewItem(ReviewDto review)
        {
            Review = review; 
            RestaurantName = review.RestaurantName;
            Comment = review.Comment ?? string.Empty;
            CommentPreview = Comment
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .FirstOrDefault(p => p.Length > 0) ?? string.Empty;
            Rating = review.Overall;

            var parts = new[] { review.Username, review.City, FormatAge(review.CreatedAt) };
            Details = string.Join(" · ", parts.Where(p => !string.IsNullOrEmpty(p)));
        }

        public string RestaurantName { get; }
        public string Details { get; }
        public string Comment { get; }
        public string CommentPreview { get; }
        public double Rating { get; }

        public ReviewDto Review { get; }

        public string CreatedAtText => Review.CreatedAt.ToLocalTime().ToString("yyyy.MM.dd HH:mm");

        // A CreatedAt UTC-ben van tárolva, ezért a UtcNow-hoz hasonlítjuk
        private static string FormatAge(DateTime createdAt)
        {
            var age = DateTime.UtcNow - createdAt;

            if (age.TotalMinutes < 1) return "épp most";
            if (age.TotalHours < 1) return $"{(int)age.TotalMinutes} perce";
            if (age.TotalDays < 1) return $"{(int)age.TotalHours} órája";
            if (age.TotalDays < 2) return "tegnap";
            return $"{(int)age.TotalDays} napja";
        }
    }
}