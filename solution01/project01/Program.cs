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
        static void CenterText(string text,
            ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            int left = (Console.WindowWidth - text.Length) / 2;
            if (left < 0) left = 0;

            try
            {
                Console.SetCursorPosition(left, Console.CursorTop);
            }
            catch { }

            Console.WriteLine(text);
            Console.ResetColor();
        }

        static string CenterInput(string text,
            ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            int left = (Console.WindowWidth - text.Length) / 2;
            if (left < 0) left = 0;

            try
            {
                Console.SetCursorPosition(left, Console.CursorTop);
            }
            catch { }

            Console.Write(text);

            Console.ResetColor();
            return Console.ReadLine();
        }

        static void SuccessMessage(string text)
        {
            Console.Clear();
            ShowLogo();
            CenterText(text, ConsoleColor.Green);
        }

        static void ErrorMessage(string text)
        {
            Console.Clear();
            ShowLogo();
            CenterText(text, ConsoleColor.Red);
        }

        static void Loading(string text)
        {
            for (int i = 0; i < 3; i++)
            {
                Console.Clear();
                ShowLogo();
                CenterText(text + new string('.', i + 1), ConsoleColor.DarkRed);
                Thread.Sleep(250);
            }
        }

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

        static void MenuBox(string[] items)
        {
            CenterText("╔══════════════════════════════╗");

            foreach (var item in items)
                CenterText($"║ {item.PadRight(29)}║");

            CenterText("╚══════════════════════════════╝");
            Console.WriteLine();
        }

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
                // 1. Browse Movies
                // =========================
                if (choice == "1")
                {
                    ShowLogo();

                    CenterText("AVAILABLE MOVIES", ConsoleColor.Cyan);
                    Console.WriteLine();

                    var movies = movieService.GetAllMovies();

                    int cardWidth = 35;
                    int spacing = 5;

                    for (int i = 0; i < movies.Count; i += 3)
                    {
                        var row = movies.Skip(i).Take(3).ToList();

                        // ================= TOP =================
                        string topLine = "";

                        foreach (var movie in row)
                        {
                            topLine += "╔" + new string('═', cardWidth - 2) + "╗";
                            topLine += new string(' ', spacing);
                        }

                        CenterText(topLine);

                        // ================= TITLE =================
                        string titleLine = "";

                        foreach (var movie in row)
                        {
                            string title = movie.Title;

                            if (title.Length > cardWidth - 4)
                                title = title.Substring(0, cardWidth - 7) + "...";

                            titleLine += $"║ {title.PadRight(cardWidth - 4)} ║";
                            titleLine += new string(' ', spacing);
                        }

                        CenterText(titleLine);

                        // ================= GENRE =================
                        string genreLine = "";

                        foreach (var movie in row)
                        {
                            string genre = "Genre: " + movie.Genre;

                            genreLine += $"║ {genre.PadRight(cardWidth - 4)} ║";
                            genreLine += new string(' ', spacing);
                        }

                        CenterText(genreLine);

                        // ================= RATING =================
                        string ratingLine = "";

                        foreach (var movie in row)
                        {
                            double avg = ratingService.GetAverageRating(movie.Id);
                            int count = ratingService.GetRatingsCount(movie.Id);

                            string rating =
                                count == 0
                                ? "Rating: No Ratings"
                                : $"Rating: {avg:F1}/5";

                            ratingLine += $"║ {rating.PadRight(cardWidth - 4)} ║";
                            ratingLine += new string(' ', spacing);
                        }

                        CenterText(ratingLine);

                        // ================= MOVIE ID =================
                        string idLine = "";

                        foreach (var movie in row)
                        {
                            string idText = $"Movie ID: {movie.Id}";

                            idLine += $"║ {idText.PadRight(cardWidth - 4)} ║";
                            idLine += new string(' ', spacing);
                        }

                        CenterText(idLine);

                        // ================= BOTTOM =================
                        string bottomLine = "";

                        foreach (var movie in row)
                        {
                            bottomLine += "╚" + new string('═', cardWidth - 2) + "╝";
                            bottomLine += new string(' ', spacing);
                        }

                        CenterText(bottomLine);

                        Console.WriteLine("\n");
                    }

                    CenterText("Press Any Key To Return...", ConsoleColor.DarkGray);
                    Console.ReadKey();
                }
                // =========================
                // 2. Search
                // =========================
                else if (choice == "2")
                {
                    ShowLogo();

                    string keyword = CenterInput("Enter Keyword: ", ConsoleColor.Yellow);

                    var results = searchService.SmartSearch(keyword);
                    searchService.DisplayResults(results);

                    Console.WriteLine("\nPress any key...");
                    Console.ReadKey();
                }

                // =========================
                // 3. Watch Movie
                // =========================
                else if (choice == "3")
                {
                    ShowLogo();

                    if (!int.TryParse(CenterInput("Enter Movie ID: ", ConsoleColor.Yellow), out int movieId))
                    {
                        ErrorMessage("Invalid Movie ID");
                        Console.ReadKey();
                        continue;
                    }

                    var movie = movieService.GetMovieById(movieId);

                    if (movie != null)
                    {
                        user.WatchHistory ??= new List<int>();

                        if (!user.WatchHistory.Contains(movieId))
                            user.WatchHistory.Add(movieId);

                        SuccessMessage("Now Watching: " + movie.Title);
                    }
                    else ErrorMessage("Movie Not Found");

                    Console.ReadKey();
                }

                // =========================
                // 4. Rate Movie
                // =========================
                else if (choice == "4")
                {
                    ShowLogo();

                    if (!int.TryParse(CenterInput("Enter Movie ID: ", ConsoleColor.Yellow), out int movieId) ||
                        !int.TryParse(CenterInput("Enter Rating (1-5): ", ConsoleColor.Yellow), out int score))
                    {
                        ErrorMessage("Invalid Input");
                        Console.ReadKey();
                        continue;
                    }

                    ratingService.AddOrUpdateRating(user.Id, movieId, score);

                    SuccessMessage("Rating Saved Successfully!");
                    Console.ReadKey();
                }

                // =========================
                // 5. Recommendations
                // =========================
                else if (choice == "5")
                {
                    ShowLogo();

                    CenterText("RECOMMENDED FOR YOU", ConsoleColor.Magenta);

                    var results = recommendationService.GetRecommendations(user);

                    if (results == null || results.Count == 0)
                    {
                        CenterText("No Recommendations Found", ConsoleColor.Red);
                    }
                    else
                    {
                        foreach (var movie in results)
                        {
                            double avg = ratingService.GetAverageRating(movie.Id);

                            if (avg == 0)
                                CenterText($"{movie.Title} | No Ratings Yet");
                            else
                                CenterText($"{movie.Title} |  {avg:F1}");
                        }
                    }

                    Console.ReadKey();
                }

                // =========================
                // 6. Watch History
                // =========================
                else if (choice == "6")
                {
                    ShowLogo();

                    CenterText("WATCH HISTORY", ConsoleColor.Cyan);

                    if (user.WatchHistory == null || user.WatchHistory.Count == 0)
                        CenterText("No Watched Movies Yet", ConsoleColor.Red);
                    else
                    {
                        foreach (var id in user.WatchHistory)
                        {
                            var movie = movieService.GetMovieById(id);
                            if (movie != null)
                                CenterText($"{movie.Title} | {movie.Genre}");
                        }
                    }

                    Console.ReadKey();
                }

                // =========================
                // 7. Logout
                // =========================
                else if (choice == "7")
                {
                    SuccessMessage("Logged Out Successfully");
                    Thread.Sleep(1500);
                    break;
                }

                else
                {
                    ErrorMessage("Invalid Option");
                    Console.ReadKey();
                }
            }
        }

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
                    string u = CenterInput("Username: ");
                    string p = CenterInput("Password: ");

                    Loading("Creating Account");

                    if (authService.Register(u, p))
                        SuccessMessage("Registered Successfully!");
                    else
                        ErrorMessage("Username already exists");

                    Thread.Sleep(1500);
                }

                else if (choice == "2")
                {
                    string u = CenterInput("Username: ");
                    string p = CenterInput("Password: ");

                    Loading("Signing In");

                    currentUser = authService.Login(u, p);

                    if (currentUser != null)
                    {
                        SuccessMessage("Login Successful!");
                        Thread.Sleep(1500);

                        UserMenu(currentUser,
                            movieService,
                            searchService,
                            ratingService,
                            recommendationService,
                            aiEngine);
                    }
                    else
                    {
                        ErrorMessage("Wrong Username or Password");
                        Thread.Sleep(1500);
                    }
                }

                else if (choice == "3")
                {
                    break;
                }

                else
                {
                    ErrorMessage("Invalid Option");
                    Console.ReadKey();
                }
            }
        }
    }
}