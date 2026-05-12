using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;

namespace MovieRecommendationSystem.Services.Recommendation
{
    public class ContentBasedService
    {
        private List<Movie> movies;
        private string movieFile = "Data/Movie.json";

        // Constructor
        public ContentBasedService()
        {
            // Load movies from JSON
            movies = FileManager.LoadData<Movie>(movieFile);
        }

        // =========================================
        // Content-Based Recommendation
        // =========================================
        public List<Movie> GetRecommendations(User user)
        {
            List<Movie> recommended = new List<Movie>();

            foreach (var movie in movies)
            {
                int score = 0;

                // Match favorite genres
                if (user.FavoriteGenres.Any(g =>
                    movie.Genre.ToLower().Contains(g.ToLower())))
                {
                    score += 3;
                }

                // Match tags
                if (movie.Tags != null &&
                    movie.Tags.Any(t =>
                        user.FavoriteGenres.Any(g =>
                            t.ToLower().Contains(g.ToLower()))))
                {
                    score += 2;
                }

                // High rating boost
                if (movie.Rating >= 4)
                {
                    score += 1;
                }

                if (score >= 3)
                {
                    recommended.Add(movie);
                }
            }

            return recommended
                .OrderByDescending(m => m.Rating)
                .ToList();
        }
    }
}