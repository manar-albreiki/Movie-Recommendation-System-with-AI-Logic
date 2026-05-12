using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;

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
        // Display Movies
        // =========================================
        public void DisplayMovies()
        {
            if (movies.Count == 0)
            {
                Console.WriteLine("No movies available.");
                return;
            }

            Console.WriteLine("\n===== Movies List =====");

            foreach (var movie in movies)
            {
                Console.WriteLine($"ID: {movie.Id}");
                Console.WriteLine($"Title: {movie.Title}");
                Console.WriteLine($"Genre: {movie.Genre}");
                Console.WriteLine($"Year: {movie.ReleaseYear}");
                Console.WriteLine($"Director: {movie.Director}");
                Console.WriteLine($"Rating: {movie.Rating}");
                Console.WriteLine("------------------------");
            }
        }
    }
}