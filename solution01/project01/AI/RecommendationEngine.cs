// RecommendationEngine.cs

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

        // Constructor
        public RecommendationEngine()
        {
            // Load data from JSON files
            movies = FileManager.LoadData<Movie>(movieFile);
            ratings = FileManager.LoadData<Rating>(ratingFile);
            users = FileManager.LoadData<User>(userFile);
        }

        // =========================================
        // Main AI Recommendation Method
        // =========================================
        public List<Movie> GenerateRecommendations(User currentUser)
        {
            // Store movie scores
            Dictionary<Movie, double> recommendationScores =
                new Dictionary<Movie, double>();

            foreach (var movie in movies)
            {
                double score = 0;

                // ---------------------------------
                // Genre Matching Score
                // ---------------------------------
                if (currentUser.FavoriteGenres.Any(g =>
                    movie.Genre.ToLower().Contains(g.ToLower())))
                {
                    score += 3;
                }

                // ---------------------------------
                // Movie Rating Score
                // ---------------------------------
                score += movie.Rating;

                // ---------------------------------
                // Collaborative Filtering Score
                // ---------------------------------
                score += GetCollaborativeScore(currentUser, movie);

                // Save score
                recommendationScores[movie] = score;
            }

            // Sort movies by score descending
            return recommendationScores
                .OrderByDescending(m => m.Value)
                .Select(m => m.Key)
                .Take(10)
                .ToList();
        }

        // =========================================
        // Collaborative Filtering Logic
        // =========================================
        private double GetCollaborativeScore(User currentUser, Movie movie)
        {
            double score = 0;

            // Current user ratings
            var currentUserRatings = ratings
                .Where(r => r.UserId == currentUser.Id)
                .ToList();

            foreach (var otherUser in users)
            {
                // Skip same user
                if (otherUser.Id == currentUser.Id)
                    continue;

                // Other user ratings
                var otherUserRatings = ratings
                    .Where(r => r.UserId == otherUser.Id)
                    .ToList();

                // Count similar rated movies
                int similarity = currentUserRatings
                    .Count(cr => otherUserRatings
                        .Any(or =>
                            or.MovieId == cr.MovieId &&
                            Math.Abs(or.Score - cr.Score) <= 1));

                // If users are similar
                if (similarity >= 2)
                {
                    // Check if similar user liked this movie
                    bool likedMovie = otherUserRatings
                        .Any(r => r.MovieId == movie.Id && r.Score >= 4);

                    if (likedMovie)
                    {
                        score += 2;
                    }
                }
            }

            return score;
        }

        // =========================================
        // Cosine Similarity
        // =========================================
        public double CalculateCosineSimilarity(
            List<double> vectorA,
            List<double> vectorB)
        {
            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vectorA.Count; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];

                magnitudeA += Math.Pow(vectorA[i], 2);

                magnitudeB += Math.Pow(vectorB[i], 2);
            }

            magnitudeA = Math.Sqrt(magnitudeA);
            magnitudeB = Math.Sqrt(magnitudeB);

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}