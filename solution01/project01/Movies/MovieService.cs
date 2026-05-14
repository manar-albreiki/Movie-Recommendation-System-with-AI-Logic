using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Services.Ratings;
using MovieRecommendationSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieRecommendationSystem.Services.Movies
{
    public class MovieService
    {
        private List<Movie> movies;
        private string filePath = "Data/Movie.json";

        public MovieService()
        {
            // تحميل الأفلام من JSON
            movies = FileManager.LoadData<Movie>(filePath);
        }

        // =========================================
        // Add Movie
        // =========================================
        public void AddMovie(Movie movie)
        {
            bool exists = movies.Any(m => m.Title.ToLower() == movie.Title.ToLower());

            if (exists)
            {
                Console.WriteLine("Movie already exists.");
                return;
            }

            movie.Id = movies.Count > 0 ? movies.Max(m => m.Id) + 1 : 1;

            movies.Add(movie);

            FileManager.SaveData(filePath, movies);

            Console.WriteLine("Movie added successfully.");
        }

        // =========================================
        // Get All Movies
        // =========================================
        public List<Movie> GetAllMovies()
        {
            return movies;
        }

        // =========================================
        // Get Movie By Id
        // =========================================
        public Movie GetMovieById(int id)
        {
            return movies.FirstOrDefault(m => m.Id == id);
        }

        // =========================================
        // Update Movie
        // =========================================
        public void UpdateMovie(int id, Movie updatedMovie)
        {
            Movie movie = GetMovieById(id);

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            movie.Title = updatedMovie.Title;
            movie.Genre = updatedMovie.Genre;
            movie.Description = updatedMovie.Description;
            movie.ReleaseYear = updatedMovie.ReleaseYear;
            movie.Director = updatedMovie.Director;
            movie.Cast = updatedMovie.Cast;
            movie.Tags = updatedMovie.Tags;
            movie.Rating = updatedMovie.Rating;

            FileManager.SaveData(filePath, movies);

            Console.WriteLine("Movie updated successfully.");
        }

        // =========================================
        // Delete Movie
        // =========================================
        public void DeleteMovie(int id)
        {
            Movie movie = GetMovieById(id);

            if (movie == null)
            {
                Console.WriteLine("Movie not found.");
                return;
            }

            movies.Remove(movie);

            FileManager.SaveData(filePath, movies);

            Console.WriteLine("Movie deleted successfully.");
        }
        // =========================================
        // DISPLAY MOVIES (FIXED RETURN)
        // =========================================
        public void DisplayMovies()
        {
            // تحميل التقييمات كل مرة عشان تكون محدثة
            var ratings = FileManager.LoadData<Rating>("Data/Rating.json") ?? new List<Rating>();

            if (movies.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nNo movies available.");
                Console.ResetColor();

               
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.ResetColor();

            int cardsPerRow = 3;

            for (int i = 0; i < movies.Count; i += cardsPerRow)
            {
                var rowMovies = movies.Skip(i).Take(cardsPerRow).ToList();

                // TOP
                foreach (var movie in rowMovies)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("╔══════════════════════════════╗   ");
                }
                Console.WriteLine();

                // TITLE
                foreach (var movie in rowMovies)
                {
                    string value = movie.Title.Length > 18
                        ? movie.Title.Substring(0, 18)
                        : movie.Title;

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"║ Title   : {value,-18} ║   ");
                }
                Console.WriteLine();

                // GENRE
                foreach (var movie in rowMovies)
                {
                    string value = movie.Genre.Length > 18
                        ? movie.Genre.Substring(0, 18)
                        : movie.Genre;

                    Console.Write($"║ Genre   : {value,-18} ║   ");
                }
                Console.WriteLine();

                // YEAR
                foreach (var movie in rowMovies)
                {
                    Console.Write($"║ Year    : {movie.ReleaseYear,-18} ║   ");
                }
                Console.WriteLine();

                // DIRECTOR
                foreach (var movie in rowMovies)
                {
                    string value = movie.Director.Length > 18
                        ? movie.Director.Substring(0, 18)
                        : movie.Director;

                    Console.Write($"║ Director: {value,-18} ║   ");
                }
                Console.WriteLine();

                // ⭐ RATING (FIX IMPORTANT PART)
                foreach (var movie in rowMovies)
                {
                    var movieRatings = ratings.Where(r => r.MovieId == movie.Id).ToList();

                    double avg = movieRatings.Count == 0
                        ? 0
                        : movieRatings.Average(r => r.Score);

                    Console.Write($"║ Rating  : {avg,-18:F1} ║   ");
                }
                Console.WriteLine();

                // ID
                foreach (var movie in rowMovies)
                {
                    Console.Write($"║ ID      : {movie.Id,-18} ║   ");
                }
                Console.WriteLine();

                // BOTTOM
                foreach (var movie in rowMovies)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("╚══════════════════════════════╝   ");
                }

                Console.WriteLine("\n");
            }

            Console.ResetColor();

            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine("\nPress any key to return...");
            //Console.ResetColor();

            //Console.ReadKey();
        }
    }
}



        