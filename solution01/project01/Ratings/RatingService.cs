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

        // Constructor
        public RatingService()
        {
            ratings = FileManager.LoadData<Rating>(ratingFile);
            movies = FileManager.LoadData<Movie>(movieFile);
        }

        // =========================================
        // Add or Update Rating
        // =========================================
        public void AddOrUpdateRating(int userId, int movieId, int score)
        {
            // =========================================
            // VALIDATE SCORE (1 to 5)
            // =========================================
            if (score < 1 || score > 5)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Rating must be between 1 and 5 only!");
                Console.ResetColor();
                return;
            }

            // =========================================
            // VALIDATE MOVIE ID (EXISTS IN JSON)
            // =========================================
            bool movieExists = movies.Any(m => m.Id == movieId);

            if (!movieExists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Invalid Movie ID! Please enter a valid movie number.");
                Console.ResetColor();
                return;
            }

            // =========================================
            // CHECK EXISTING RATING
            // =========================================
            Rating existingRating = ratings
                .FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);

            if (existingRating != null)
            {
                existingRating.Score = score;
                existingRating.RatedAt = DateTime.Now;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("✔ Rating updated successfully.");
                Console.ResetColor();
            }
            else
            {
                Rating newRating = new Rating
                {
                    Id = ratings.Count > 0 ? ratings.Max(r => r.Id) + 1 : 1,
                    UserId = userId,
                    MovieId = movieId,
                    Score = score,
                    RatedAt = DateTime.Now
                };

                ratings.Add(newRating);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✔ Rating added successfully.");
                Console.ResetColor();
            }

            FileManager.SaveData(ratingFile, ratings);

            UpdateMovieRating(movieId);
        }

        // =========================================
        // Remove Rating
        // =========================================
        public void RemoveRating(int userId, int movieId)
        {
            var rating = ratings
                .FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);

            if (rating == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Rating not found.");
                Console.ResetColor();
                return;
            }

            ratings.Remove(rating);
            FileManager.SaveData(ratingFile, ratings);

            UpdateMovieRating(movieId);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Rating removed successfully.");
            Console.ResetColor();
        }

        // =========================================
        // Get Movie Ratings
        // =========================================
        public List<Rating> GetMovieRatings(int movieId)
        {
            return ratings.Where(r => r.MovieId == movieId).ToList();
        }

        // =========================================
        // Get Average Rating
        // =========================================
        public double GetAverageRating(int movieId)
        {
            var movieRatings = ratings.Where(r => r.MovieId == movieId);

            if (!movieRatings.Any())
                return 0;

            return movieRatings.Average(r => r.Score);
        }

        // =========================================
        // Update Movie Rating
        // =========================================
        private void UpdateMovieRating(int movieId)
        {
            Movie movie = movies.FirstOrDefault(m => m.Id == movieId);

            if (movie == null)
                return;

            movie.Rating = GetAverageRating(movieId);

            FileManager.SaveData(movieFile, movies);
        }
    }
}