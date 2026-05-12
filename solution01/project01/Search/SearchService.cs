using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;

namespace MovieRecommendationSystem.Services.Search
{
    public class SearchService
    {
        private List<Movie> movies;
        private string filePath = "Data/Movie.json";

        public SearchService()
        {
            // تحميل الأفلام من JSON
            movies = FileManager.LoadData<Movie>(filePath);
        }

        // =========================================
        // Search By Title
        // =========================================
        public List<Movie> SearchByTitle(string title)
        {
            return movies
                .Where(m => m.Title.ToLower().Contains(title.ToLower()))
                .ToList();
        }

        // =========================================
        // Search By Genre
        // =========================================
        public List<Movie> SearchByGenre(string genre)
        {
            return movies
                .Where(m => m.Genre.ToLower().Contains(genre.ToLower()))
                .ToList();
        }

        // =========================================
        // Search By Year
        // =========================================
        public List<Movie> SearchByYear(int year)
        {
            return movies
                .Where(m => m.ReleaseYear == year)
                .ToList();
        }

        // =========================================
        // Search By Director
        // =========================================
        public List<Movie> SearchByDirector(string director)
        {
            return movies
                .Where(m => m.Director.ToLower().Contains(director.ToLower()))
                .ToList();
        }

        // =========================================
        // Search By Rating
        // =========================================
        public List<Movie> SearchByRating(double rating)
        {
            return movies
                .Where(m => m.Rating >= rating)
                .OrderByDescending(m => m.Rating)
                .ToList();
        }

        // =========================================
        // Smart Search (Advanced)
        // =========================================
        public List<Movie> SmartSearch(string keyword)
        {
            return movies
                .Where(m =>
                    m.Title.ToLower().Contains(keyword.ToLower()) ||
                    m.Genre.ToLower().Contains(keyword.ToLower()) ||
                    m.Director.ToLower().Contains(keyword.ToLower()) ||
                    m.Tags.Any(t => t.ToLower().Contains(keyword.ToLower()))
                )
                .ToList();
        }

        // =========================================
        // Display Results
        // =========================================
        public void DisplayResults(List<Movie> results)
        {
            if (results == null || results.Count == 0)
            {
                Console.WriteLine("No movies found.");
                return;
            }

            Console.WriteLine("\n===== Search Results =====");

            foreach (var movie in results)
            {
                Console.WriteLine($"ID: {movie.Id}");
                Console.WriteLine($"Title: {movie.Title}");
                Console.WriteLine($"Genre: {movie.Genre}");
                Console.WriteLine($"Year: {movie.ReleaseYear}");
                Console.WriteLine($"Director: {movie.Director}");
                Console.WriteLine($"Rating: {movie.Rating}");
                Console.WriteLine("----------------------------");
            }
        }
    }
}