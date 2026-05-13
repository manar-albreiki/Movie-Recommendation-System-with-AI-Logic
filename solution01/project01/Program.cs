using MovieRecommendationSystem.AI;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Services.Authentication;
using MovieRecommendationSystem.Services.Movies;
using MovieRecommendationSystem.Services.Ratings;
using MovieRecommendationSystem.Services.Recommendation;
using MovieRecommendationSystem.Services.Search;
using MovieRecommendationSystem.Utilities;

using System;
using System.Linq;
using System.Collections.Generic;

namespace MovieRecommendationSystem
{
    class Program
    {
        static void UserMenu(
            User user,
            MovieService movieService,
            SearchService searchService,
            RatingService ratingService,
            RecommendationService recommendationService,
            RecommendationEngine aiEngine)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n===== Welcome ({user.Username}) =====");
                Console.ResetColor();

                Console.WriteLine("1. Browse Movies");
                Console.WriteLine("2. Search Movies");
                Console.WriteLine("3. Watch Movie");
                Console.WriteLine("4. Rate Movie");
                Console.WriteLine("5. View Recommendations");
                Console.WriteLine("6. Watch History");
                Console.WriteLine("7. Logout");

                Console.Write("\nChoose: ");

                string choice = Console.ReadLine();

                Console.WriteLine();

                // =========================================
                // 1. Browse Movies
                // =========================================
                if (choice == "1")
                {
                    movieService.DisplayMovies();
                }

                // =========================================
                // 2. Search Movies
                // =========================================
                else if (choice == "2")
                {
                    Console.Write("Enter keyword: ");
                    string keyword = Console.ReadLine();

                    var results = searchService.SmartSearch(keyword);

                    searchService.DisplayResults(results);
                }

                // =========================================
                // 3. Watch Movie
                // =========================================
                else if (choice == "3")
                {
                    Console.Write("Enter Movie ID to watch: ");
                    int movieId = int.Parse(Console.ReadLine());

                    var movie = movieService.GetMovieById(movieId);

                    if (movie != null)
                    {
                        // create history list if null
                        if (user.WatchHistory == null)
                        {
                            user.WatchHistory = new List<int>();
                        }

                        // add only if not already watched
                        if (!user.WatchHistory.Contains(movieId))
                        {
                            user.WatchHistory.Add(movieId);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"You watched: {movie.Title}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Movie not found.");
                        Console.ResetColor();
                    }
                }
                // =========================================
                // 4. Rate Movie
                // =========================================
                else if (choice == "4")
                {
                    Console.Write("Enter Movie ID: ");

                    int movieId =
                        int.Parse(Console.ReadLine());

                    Console.Write("Enter Rating (1-5): ");

                    int score =
                        int.Parse(Console.ReadLine());

                    ratingService.AddOrUpdateRating(
                        user.Id,
                        movieId,
                        score);
                }

                // =========================================
                // 5. Recommendations
                // =========================================
                else if (choice == "5")
                {
                    var results =
                        recommendationService
                        .GetRecommendations(user);

                    Console.ForegroundColor =
                        ConsoleColor.Yellow;

                    Console.WriteLine(
                        "===== RECOMMENDATIONS =====");

                    Console.ResetColor();

                    foreach (var movie in results)
                    {
                        Console.WriteLine(
                            $"{movie.Title} | {movie.Rating:F1}");
                    }
                }

                // =========================================
                // 6. Watch History
                // =========================================
                else if (choice == "6")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("===== WATCH HISTORY =====");
                    Console.ResetColor();

                    if (user.WatchHistory == null || user.WatchHistory.Count == 0)
                    {
                        Console.WriteLine("No watched movies yet.");
                    }
                    else
                    {
                        foreach (var movieId in user.WatchHistory)
                        {
                            var movie = movieService.GetMovieById(movieId);

                            if (movie != null)
                            {
                                Console.WriteLine(
                                    $"ID: {movie.Id} | {movie.Title} | {movie.Genre}"
                                );
                            }
                        }
                    }
                }
                // =========================================
                // 7. Logout
                // =========================================
                else if (choice == "7")
                {
                    Console.WriteLine(
                        "Logged out successfully.");

                    break;
                }

                // =========================================
                // Invalid Option
                // =========================================
                else
                {
                    Console.ForegroundColor =
                        ConsoleColor.Red;

                    Console.WriteLine(
                        "Invalid option, try again.");

                    Console.ResetColor();
                }
            }
        }

        static void Main(string[] args)
        {
            AuthenticationService authService =
                new AuthenticationService(
                    FileManager.LoadData<User>(
                        "Data/User.json"));

            MovieService movieService =
                new MovieService();

            SearchService searchService =
                new SearchService();

            RatingService ratingService =
                new RatingService();

            RecommendationService recommendationService =
                new RecommendationService();

            RecommendationEngine aiEngine =
                new RecommendationEngine();

            User currentUser = null;

            while (true)
            {
                Console.ForegroundColor =
                    ConsoleColor.Cyan;

                Console.WriteLine(
                    "\n===== Movie Recommendation System =====");

                Console.ResetColor();

                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                Console.Write("Choose: ");

                string choice = Console.ReadLine();

                // =========================================
                // REGISTER
                // =========================================
                if (choice == "1")
                {
                    Console.Write("Enter username: ");

                    string username =
                        Console.ReadLine();

                    Console.Write("Enter password: ");

                    string password =
                        Console.ReadLine();

                    authService.Register(
                        username,
                        password);
                }

                // =========================================
                // LOGIN
                // =========================================
                else if (choice == "2")
                {
                    Console.Write("Enter username: ");

                    string username =
                        Console.ReadLine();

                    Console.Write("Enter password: ");

                    string password =
                        Console.ReadLine();

                    currentUser =
                        authService.Login(
                            username,
                            password);

                    if (currentUser != null)
                    {
                        UserMenu(
                            currentUser,
                            movieService,
                            searchService,
                            ratingService,
                            recommendationService,
                            aiEngine);
                    }
                }

                // =========================================
                // EXIT
                // =========================================
                else if (choice == "3")
                {
                    Console.WriteLine("Goodbye");
                    break;
                }

                // =========================================
                // INVALID OPTION
                // =========================================
                else
                {
                    Console.ForegroundColor =
                        ConsoleColor.Red;

                    Console.WriteLine(
                        "Invalid option.");

                    Console.ResetColor();
                }
            }
        }
    }
}