using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieRecommendationSystem.Services.Ratings
{
    public class RatingService
    {
        private List<Rating> ratings;
        private string filePath = "Data/Rating.json";

        public RatingService()
        {
            ratings = FileManager.LoadData<Rating>(filePath) ?? new List<Rating>();
        }

        // ======================
        // ADD OR UPDATE RATING
        // ======================
        public void AddOrUpdateRating(int userId, int movieId, int score)
        {
            var existing = ratings.FirstOrDefault(r =>
                r.UserId == userId && r.MovieId == movieId);

            if (existing != null)
            {
                existing.Score = score;
                existing.RatedAt = DateTime.Now;
            }
            else
            {
                ratings.Add(new Rating
                {
                    Id = ratings.Count > 0 ? ratings.Max(r => r.Id) + 1 : 1,
                    UserId = userId,
                    MovieId = movieId,
                    Score = score,
                    RatedAt = DateTime.Now
                });
            }

            FileManager.SaveData(filePath, ratings);
        }

        // ======================
        // AVERAGE RATING
        // ======================
        public double GetAverageRating(int movieId)
        {
            var list = ratings.Where(r => r.MovieId == movieId).ToList();

            if (list.Count == 0)
                return 0;

            return list.Average(r => r.Score);
        }

        // ======================
        // COUNT RATINGS (IMPORTANT FIX)
        // ======================
        public int GetRatingsCount(int movieId)
        {
            return ratings.Count(r => r.MovieId == movieId);
        }

        // ======================
        // OPTIONAL HELPERS
        // ======================
        public List<Rating> GetAllRatings()
        {
            return ratings;
        }

        public Rating GetUserRating(int userId, int movieId)
        {
            return ratings.FirstOrDefault(r =>
                r.UserId == userId && r.MovieId == movieId);
        }
    }
}