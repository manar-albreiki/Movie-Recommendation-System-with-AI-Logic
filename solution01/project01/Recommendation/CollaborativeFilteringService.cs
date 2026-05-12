using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;

namespace MovieRecommendationSystem.Services.Recommendation
{
    public class CollaborativeFilteringService
    {
        private List<User> users;
        private List<Rating> ratings;
        private List<Movie> movies;

        private string userFile = "Data/User.json";
        private string ratingFile = "Data/Rating.json";
        private string movieFile = "Data/Movie.json";

        // Constructor
        public CollaborativeFilteringService()
        {
            // Load data from JSON
            users = FileManager.LoadData<User>(userFile);
            ratings = FileManager.LoadData<Rating>(ratingFile);
            movies = FileManager.LoadData<Movie>(movieFile);
        }

        // =========================================
        // Collaborative Filtering
        // =========================================
        public List<Movie> GetRecommendations(User user)
        {
            List<Movie> recommended = new List<Movie>();

            // Get movies rated by current user
            var userRatings = ratings.Where(r => r.UserId == user.Id).ToList();

            foreach (var otherUser in users)
            {
                if (otherUser.Id == user.Id)
                    continue;

                // Get common rated movies
                var otherRatings = ratings.Where(r => r.UserId == otherUser.Id);

                int commonCount = userRatings
                    .Count(ur => otherRatings.Any(or => or.MovieId == ur.MovieId));

                // If users are similar
                if (commonCount >= 2)
                {
                    // Get movies liked by similar user
                    var likedMovies = otherRatings
                        .Where(r => r.Score >= 4)
                        .Select(r => r.MovieId);

                    foreach (var movieId in likedMovies)
                    {
                        var movie = movies.FirstOrDefault(m => m.Id == movieId);

                        if (movie != null &&
                            !recommended.Any(m => m.Id == movie.Id))
                        {
                            recommended.Add(movie);
                        }
                    }
                }
            }

            return recommended;
        }
    }
}