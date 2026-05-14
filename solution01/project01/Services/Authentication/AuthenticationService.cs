using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.Utilities;

namespace MovieRecommendationSystem.Services.Authentication
{
    public class AuthenticationService
    {
        private List<User> users;

        // 🔥 FIXED: absolute safe path
        private string filePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "User.json");

        // Constructor
        public AuthenticationService(List<User> usersList)
        {
            users = usersList ?? new List<User>();
        }

        // ================================
        // REGISTER
        // ================================
        public bool Register(string username, string password)
        {
            // 🔹 Ensure Data folder exists (IMPORTANT FIX)
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // 🔹 Load latest users
            users = FileManager.LoadData<User>(filePath);

            // 🔹 Validate username
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Username cannot be empty.");
                Console.ResetColor();
                return false;
            }

            username = username.Trim();

            // 🔹 Check duplicate username
            if (users.Any(u => u.Username.ToLower() == username.ToLower()))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Username already exists.");
                Console.ResetColor();
                return false;
            }

            // 🔹 Validate password (6 digits only)
            if (!IsValidPassword(password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Password must be exactly 6 digits (numbers only).");
                Console.ResetColor();
                return false;
            }

            // 🔹 Create user
            User newUser = new User
            {
                Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
                Username = username,
                Password = password,
                FavoriteGenres = new List<string>(),
                WatchHistory = new List<int>(),
                Ratings = new List<Rating>()
            };

            users.Add(newUser);

            // 🔥 SAVE DIRECTLY TO JSON
            FileManager.SaveData(filePath, users);

            

            return true;
        }

        // ================================
        // LOGIN
        // ================================
        public User Login(string username, string password)
        {
            users = FileManager.LoadData<User>(filePath);

            var loggedUser = users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password);

            if (loggedUser != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Login successful.");
                Console.ResetColor();

                return loggedUser;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid username or password.");
            Console.ResetColor();

            return null;
        }

        // ================================
        // LOGOUT
        // ================================
        public void Logout(User user)
        {
            if (user != null)
            {
                Console.WriteLine($"{user.Username} logged out successfully.");
            }
        }

        // ================================
        // PASSWORD VALIDATION
        // ================================
        private bool IsValidPassword(string password)
        {
            return password.Length == 6 && password.All(char.IsDigit);
        }
    }
}