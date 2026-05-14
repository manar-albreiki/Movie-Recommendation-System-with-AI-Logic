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
using System.Threading;

namespace MovieRecommendationSystem
{
    class Program
    {
        // =========================================
        // CENTER TEXT
        // =========================================
        static void CenterText(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            int left = (Console.WindowWidth - text.Length) / 2;
            if (left < 0) left = 0;

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(text);

            Console.ResetColor();
        }

        // =========================================
        // CENTER INPUT
        // =========================================
        static string CenterInput(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            int left = (Console.WindowWidth - text.Length) / 2;
            if (left < 0) left = 0;

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write(text);

            Console.ResetColor();

            return Console.ReadLine();
        }

        // =========================================
        // SUCCESS
        // =========================================
        static void SuccessMessage(string text)
        {
            Console.Clear();
            ShowLogo();
            CenterText(text, ConsoleColor.Green);
        }

        // =========================================
        // ERROR
        // =========================================
        static void ErrorMessage(string text)
        {
            Console.Clear();
            ShowLogo();
            CenterText(text, ConsoleColor.Red);
        }

        // =========================================
        // LOADING (بدون توقف تلقائي بعده)
        // =========================================
        static void Loading(string text)
        {
            for (int i = 0; i < 3; i++)
            {
                Console.Clear();
                ShowLogo();
                CenterText(text + new string('.', i + 1), ConsoleColor.DarkRed);
                Thread.Sleep(200);
            }
        }

        // =========================================
        // LOGO
        // =========================================
        static void ShowLogo()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;

            CenterText(@"███╗   ███╗ ██████╗ ██╗   ██╗██╗███████╗");
            CenterText(@"████╗ ████║██╔═══██╗██║   ██║██║██╔════╝");
            CenterText(@"██╔████╔██║██║   ██║██║   ██║██║█████╗  ");
            CenterText(@"██║╚██╔╝██║██║   ██║╚██╗ ██╔╝██║██╔══╝  ");
            CenterText(@"██║ ╚═╝ ██║╚██████╔╝ ╚████╔╝ ██║███████╗");
            CenterText(@"╚═╝     ╚═╝ ╚═════╝   ╚═══╝  ╚═╝╚══════╝");

            Console.WriteLine();
            CenterText("MOVIE RECOMMENDATION SYSTEM", ConsoleColor.Red);
            Console.WriteLine();
        }

        // =========================================
        // MENU BOX
        // =========================================
        static void MenuBox(string[] items)
        {
            CenterText("╔══════════════════════════════╗");

            foreach (var item in items)
            {
                CenterText($"║ {item.PadRight(29)}║", ConsoleColor.White);
            }

            CenterText("╚══════════════════════════════╝");
            Console.WriteLine();
        }

        // =========================================
        // USER MENU
        // =========================================
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
                ShowLogo();

                CenterText($"Welcome {user.Username}", ConsoleColor.Green);

                MenuBox(new string[]
                {
                    "1. Browse Movies",
                    "2. Search Movies",
                    "3. Watch Movie",
                    "4. Rate Movie",
                    "5. Recommendations",
                    "6. Watch History",
                    "7. Logout"
                });

                string choice = CenterInput("Choose Option: ", ConsoleColor.Yellow);

                // =========================
                // 1 BROWSE
                // =========================
                if (choice == "1")
                {
                    ShowLogo();
                    CenterText("AVAILABLE MOVIES", ConsoleColor.Cyan);

                    movieService.DisplayMovies();

                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                }

                // =========================
                // 2 SEARCH
                // =========================
                else if (choice == "2")
                {
                    ShowLogo();

                    string keyword = CenterInput("Enter Keyword: ", ConsoleColor.Yellow);

                    var results = searchService.SmartSearch(keyword);

                    searchService.DisplayResults(results);

                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                }

                // =========================
                // 3 WATCH
                // =========================
                else if (choice == "3")
                {
                    ShowLogo();

                    int movieId = int.Parse(CenterInput("Enter Movie ID: ", ConsoleColor.Yellow));

                    var movie = movieService.GetMovieById(movieId);

                    if (movie != null)
                    {
                        user.WatchHistory ??= new List<int>();

                        if (!user.WatchHistory.Contains(movieId))
                            user.WatchHistory.Add(movieId);

                        SuccessMessage("Now Watching: " + movie.Title);
                    }
                    else
                    {
                        ErrorMessage("Movie Not Found");
                    }

                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                }

                // =========================
                // 4 RATE
                // =========================
                else if (choice == "4")
                {
                    ShowLogo();

                    int movieId = int.Parse(CenterInput("Enter Movie ID: ", ConsoleColor.Yellow));
                    int score = int.Parse(CenterInput("Enter Rating (1-5): ", ConsoleColor.Yellow));

                    ratingService.AddOrUpdateRating(user.Id, movieId, score);

                    SuccessMessage("Rating Saved Successfully!");

                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                }

                // =========================
                // 5 RECOMMEND
                // =========================
                else if (choice == "5")
                {
                    ShowLogo();

                    CenterText("RECOMMENDED FOR YOU", ConsoleColor.Magenta);

                    var results = recommendationService.GetRecommendations(user);

                    foreach (var movie in results)
                        CenterText($"{movie.Title} | {movie.Rating:F1}");

                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                }

                // =========================
                // 6 HISTORY
                // =========================
                else if (choice == "6")
                {
                    ShowLogo();

                    CenterText("WATCH HISTORY", ConsoleColor.Cyan);

                    if (user.WatchHistory == null || user.WatchHistory.Count == 0)
                    {
                        ErrorMessage("No Watched Movies Yet");
                    }
                    else
                    {
                        foreach (var movieId in user.WatchHistory)
                        {
                            var movie = movieService.GetMovieById(movieId);

                            if (movie != null)
                                CenterText($"{movie.Title} | {movie.Genre}");
                        }
                    }

                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                }

                // =========================
                // LOGOUT
                // =========================
                else if (choice == "7")
                {
                    SuccessMessage("Logged Out Successfully");
                    Thread.Sleep(2000);
                    break;
                }

                else
                {
                    ErrorMessage("Invalid Option");
                    Console.WriteLine("\nPress any key to retry...");
                    Console.ReadKey();
                }
            }
        }

        // =========================================
        // MAIN
        // =========================================
        static void Main(string[] args)
        {
            Console.Title = "MOVIE SYSTEM";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            var authService = new AuthenticationService(
                FileManager.LoadData<User>("Data/User.json"));

            MovieService movieService = new MovieService();
            SearchService searchService = new SearchService();
            RatingService ratingService = new RatingService();
            RecommendationService recommendationService = new RecommendationService();
            RecommendationEngine aiEngine = new RecommendationEngine();

            User currentUser = null;

            while (true)
            {
                ShowLogo();

                MenuBox(new string[]
                {
                    "1. Register",
                    "2. Login",
                    "3. Exit"
                });

                string choice = CenterInput("Choose Option: ", ConsoleColor.Yellow);

                if (choice == "1")
                {
                    ShowLogo();

                    string username = CenterInput("Enter Username: ");
                    string password = CenterInput("Enter Password: ");

                    Loading("Creating Account");

                    bool success = authService.Register(username, password);

                    if (success)
                    {
                        SuccessMessage("Registered Successfully!");

                        Thread.Sleep(2000); // يدخل تلقائي بعد 2 ثانية
                    }
                    else
                    {
                        ErrorMessage("Username already exists");

                        Thread.Sleep(2000); // يرجع للمينيو تلقائي
                    }
                }

                else if (choice == "2")
                {
                    ShowLogo();

                    string username = CenterInput("Enter Username: ");
                    string password = CenterInput("Enter Password: ");

                    Loading("Signing In");

                    currentUser = authService.Login(username, password);

                    if (currentUser != null)
                    {
                        SuccessMessage("Login Successful!");

                        Thread.Sleep(2000); // يدخل تلقائي للصفحة الثانية

                        UserMenu(
                            currentUser,
                            movieService,
                            searchService,
                            ratingService,
                            recommendationService,
                            aiEngine);
                    }
                    else
                    {
                        ErrorMessage("Wrong Username or Password");

                        Thread.Sleep(2000); // يرجع للمينيو الرئيسي
                    }
                }

                else if (choice == "3")
                {
                    Loading("Closing");
                    CenterText("GOODBYE", ConsoleColor.Red);
                    Console.ReadKey();
                    break;
                }

                else
                {
                    ErrorMessage("Invalid Option");
                    Console.WriteLine("\nPress any key...");
                    Console.ReadKey();
                }
            }
        }
    }
}