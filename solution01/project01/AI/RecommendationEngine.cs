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
        // CONSTRUCTOR
        // =========================================
        public RecommendationEngine()
        {
            LoadData();
        }

        // =========================================
        // LOAD DATA
        // =========================================
        private void LoadData()
        {
            movies = FileManager.LoadData<Movie>(movieFile)
                     ?? new List<Movie>();

            ratings = FileManager.LoadData<Rating>(ratingFile)
                      ?? new List<Rating>();

            users = FileManager.LoadData<User>(userFile)
                    ?? new List<User>();
        }

        // =========================================
        // MAIN RECOMMENDATION SYSTEM
        // =========================================
        public List<Movie> GenerateRecommendations(User currentUser)
        {
            LoadData();

            List<MovieScore> scores =
                new List<MovieScore>();

            // =========================================
            // USER RATINGS
            // =========================================
            var userRatings = ratings
                .Where(r => r.UserId == currentUser.Id)
                .ToList();

            // =========================================
            // USER FAVORITE GENRES
            // =========================================
            var favoriteGenres = userRatings
                .Join(
                    movies,
                    rating => rating.MovieId,
                    movie => movie.Id,
                    (rating, movie) => new { rating, movie }
                )
                .Where(x => x.rating.Score >= 4)
                .Select(x => x.movie.Genre)
                .Distinct()
                .ToList();

            // =========================================
            // USER FAVORITE MOVIES
            // =========================================
            var likedMovieIds = userRatings
                .Where(r => r.Score >= 4)
                .Select(r => r.MovieId)
                .ToList();

            foreach (var movie in movies)
            {
                // =========================================
                // SKIP WATCHED MOVIES
                // =========================================
                if (currentUser.WatchHistory != null &&
                    currentUser.WatchHistory.Contains(movie.Id))
                {
                    continue;
                }

                double score = 0;

                // =========================================
                // GENRE BONUS
                // =========================================
                if (favoriteGenres.Contains(movie.Genre))
                {
                    score += 5;
                }

                // =========================================
                // TAG MATCH BONUS
                // =========================================
                foreach (var likedMovieId in likedMovieIds)
                {
                    var likedMovie = movies
                        .FirstOrDefault(m => m.Id == likedMovieId);

                    if (likedMovie == null)
                        continue;

                    if (movie.Tags != null &&
                        likedMovie.Tags != null)
                    {
                        int commonTags =
                            movie.Tags
                            .Intersect(likedMovie.Tags)
                            .Count();

                        score += commonTags * 2;
                    }
                }

                // =========================================
                // MOVIE GLOBAL RATING BONUS
                // =========================================
                var movieRatings = ratings
                    .Where(r => r.MovieId == movie.Id)
                    .ToList();

                double averageRating = 0;

                if (movieRatings.Count > 0)
                {
                    averageRating =
                        movieRatings.Average(r => r.Score);

                    score += averageRating;
                }

                // =========================================
                // SAVE ACTUAL MOVIE RATING
                // =========================================
                movie.Rating =
                    Math.Round(averageRating, 1);

                // =========================================
                // EXTRA BONUS FOR HIGH RATINGS
                // =========================================
                if (averageRating >= 4)
                {
                    score += 2;
                }

                // =========================================
                // ADD RESULT
                // =========================================
                scores.Add(new MovieScore
                {
                    Movie = movie,
                    Score = score
                });
            }

            // =========================================
            // RETURN BEST MOVIES
            // =========================================
            return scores
                .OrderByDescending(x => x.Score)
                .Take(10)
                .Select(x => x.Movie)
                .ToList();
        }
    }

    // =========================================
    // HELPER CLASS
    // =========================================
    public class MovieScore
    {
        public Movie Movie { get; set; }

        public double Score { get; set; }
    }
}