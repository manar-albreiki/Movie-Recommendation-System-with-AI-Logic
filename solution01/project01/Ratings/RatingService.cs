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
            // Load ratings and movies from JSON files
            ratings = FileManager.LoadData<Rating>(ratingFile);
            movies = FileManager.LoadData<Movie>(movieFile);
        }

        // =========================================
        // Add or Update Rating
        // =========================================
        public void AddOrUpdateRating(int userId, int movieId, int score)
        {
            // Check if the user already rated this movie
            Rating existingRating = ratings
                .FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);

            if (existingRating != null)
            {
                // Update existing rating
                existingRating.Score = score;
                existingRating.RatedAt = DateTime.Now;

                Console.WriteLine("Rating updated successfully.");
            }
            else
            {
                // Create new rating
                Rating newRating = new Rating
                {
                    Id = ratings.Count > 0 ? ratings.Max(r => r.Id) + 1 : 1,
                    UserId = userId,
                    MovieId = movieId,
                    Score = score,
                    RatedAt = DateTime.Now
                };

                ratings.Add(newRating);

                Console.WriteLine("Rating added successfully.");
            }

            // Save updated ratings to JSON
            FileManager.SaveData(ratingFile, ratings);

            // Update movie average rating
            UpdateMovieRating(movieId);
        }

        // =========================================
        // Remove Rating
        // =========================================
        public void RemoveRating(int userId, int movieId)
        {
            // Find rating by user and movie
            var rating = ratings
                .FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);

            if (rating == null)
            {
                Console.WriteLine("Rating not found.");
                return;
            }

            // Remove rating from list
            ratings.Remove(rating);

            // Save changes to JSON
            FileManager.SaveData(ratingFile, ratings);

            // Update movie rating after removal
            UpdateMovieRating(movieId);

            Console.WriteLine("Rating removed successfully.");
        }

        // =========================================
        // Get Movie Ratings
        // =========================================
        public List<Rating> GetMovieRatings(int movieId)
        {
            // Return all ratings for a specific movie
            return ratings.Where(r => r.MovieId == movieId).ToList();
        }

        // =========================================
        // Calculate Average Rating
        // =========================================
        public double GetAverageRating(int movieId)
        {
            // Get all ratings for the movie
            var movieRatings = ratings.Where(r => r.MovieId == movieId);

            // If no ratings exist, return 0
            if (!movieRatings.Any())
                return 0;

            // Calculate average score
            return movieRatings.Average(r => r.Score);
        }

        // =========================================
        // Update Movie Rating in Movie JSON
        // =========================================
        private void UpdateMovieRating(int movieId)
        {
            // Find the movie
            Movie movie = movies.FirstOrDefault(m => m.Id == movieId);

            if (movie == null)
                return;

            // Update movie rating with average rating
            movie.Rating = GetAverageRating(movieId);

            // Save updated movies to JSON
            FileManager.SaveData(movieFile, movies);
        }
    }
}