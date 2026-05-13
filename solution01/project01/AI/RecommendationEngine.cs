using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;

namespace MovieRecommendationSystem.AI
{
    public class RecommendationEngine
    {
        private List<Movie> movies;
        private List<Rating> ratings;
        private List<User> users;

        private string movieFile = "Data/Movie.json";
        private string ratingFile = "Data/Rating.json";
        private string userFile = "Data/User.json";

        // =========================================
        // Constructor
        // =========================================
        public RecommendationEngine()
        {
            movies = FileManager.LoadData<Movie>(movieFile);
            ratings = FileManager.LoadData<Rating>(ratingFile);
            users = FileManager.LoadData<User>(userFile);
        }

        // =========================================
        // MAIN AI RECOMMENDATION METHOD
        // =========================================
        public List<Movie> GenerateRecommendations(User currentUser)
        {
            // reload latest data
            movies = FileManager.LoadData<Movie>(movieFile);
            ratings = FileManager.LoadData<Rating>(ratingFile);
            users = FileManager.LoadData<User>(userFile);

            List<Movie> recommendedMovies = new List<Movie>();

            foreach (var movie in movies)
            {
                double contentScore = CalculateContentScore(currentUser, movie);

                double collaborativeScore =
                    CalculateCollaborativeScore(currentUser, movie);

                double finalScore =
                    (contentScore * 0.6) +
                    (collaborativeScore * 0.4);

                // IMPORTANT FIX
                movie.Rating = finalScore;

                recommendedMovies.Add(movie);
            }

            return recommendedMovies
                .OrderByDescending(m => m.Rating)
                .Take(10)
                .ToList();
        }

        // =========================================
        // CONTENT-BASED FILTERING
        // =========================================
        private double CalculateContentScore(User user, Movie movie)
        {
            double score = 0;

            if (user.FavoriteGenres != null)
            {
                foreach (var genre in user.FavoriteGenres)
                {
                    if (movie.Genre.ToLower()
                        .Contains(genre.ToLower()))
                    {
                        score += 3;
                    }
                }
            }

            return score;
        }

        // =========================================
        // COLLABORATIVE FILTERING
        // =========================================
        private double CalculateCollaborativeScore(
            User currentUser,
            Movie movie)
        {
            double score = 0;

            var movieRatings = ratings
                .Where(r => r.MovieId == movie.Id)
                .ToList();

            if (movieRatings.Count > 0)
            {
                score = movieRatings.Average(r => r.Score);
            }

            return score;
        }
    }
}