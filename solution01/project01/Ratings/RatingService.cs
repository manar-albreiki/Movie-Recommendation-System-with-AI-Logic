using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;

namespace MovieRecommendationSystem.Services.Ratings
{
    public class RatingService
    {
        private List<Rating> ratings;
        private List<Movie> movies;

        private string ratingFile = "Data/Rating.json";
        private string movieFile = "Data/Movie.json";

        public RatingService()
        {
            LoadData();
        }

        // =========================================
        // LOAD DATA
        // =========================================
        private void LoadData()
        {
            ratings = FileManager.LoadData<Rating>(ratingFile) ?? new List<Rating>();
            movies = FileManager.LoadData<Movie>(movieFile) ?? new List<Movie>();
        }

        // =========================================
        // ADD OR UPDATE RATING
        // =========================================
        public void AddOrUpdateRating(int userId, int movieId, int score)
        {
            LoadData();

            if (score < 1 || score > 5)
            {
                Console.WriteLine("Rating must be between 1 and 5 only!");
                return;
            }

            if (!movies.Any(m => m.Id == movieId))
            {
                Console.WriteLine("Invalid Movie ID!");
                return;
            }

            var existing = ratings
                .FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);

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

            FileManager.SaveData(ratingFile, ratings);

            UpdateMovieRating(movieId);
        }

        // =========================================
        // GET AVERAGE RATING (FIXED)
        // =========================================
        public double GetAverageRating(int movieId)
        {
            LoadData();

            var movieRatings = ratings
                .Where(r => r.MovieId == movieId)
                .Select(r => r.Score)
                .ToList();

            if (movieRatings.Count == 0)
                return 0;

            return Math.Round(movieRatings.Average(), 1);
        }

        // =========================================
        // UPDATE MOVIE RATING (FIXED)
        // =========================================
        private void UpdateMovieRating(int movieId)
        {
            LoadData();

            var movie = movies.FirstOrDefault(m => m.Id == movieId);

            if (movie == null)
                return;

            var movieRatings = ratings
                .Where(r => r.MovieId == movieId)
                .Select(r => r.Score)
                .ToList();

            movie.Rating = movieRatings.Count > 0
                ? Math.Round(movieRatings.Average(), 1)
                : 0;

            FileManager.SaveData(movieFile, movies);
        }

        // =========================================
        // GET MOVIE RATINGS
        // =========================================
        public List<Rating> GetMovieRatings(int movieId)
        {
            LoadData();
            return ratings.Where(r => r.MovieId == movieId).ToList();
        }

        // =========================================
        // REMOVE RATING
        // =========================================
        public void RemoveRating(int userId, int movieId)
        {
            LoadData();

            var rating = ratings
                .FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);

            if (rating == null)
            {
                Console.WriteLine("Rating not found.");
                return;
            }

            ratings.Remove(rating);
            FileManager.SaveData(ratingFile, ratings);

            UpdateMovieRating(movieId);
        }
    }
}