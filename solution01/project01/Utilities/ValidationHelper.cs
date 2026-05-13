using System;
using System.Text.RegularExpressions;

namespace MovieRecommendationSystem.Utilities
{
    public static class ValidationHelper
    {
        // =========================================
        // Validate Username
        // =========================================
        public static bool IsValidUsername(string username)
        {
            // Username must not be empty and at least 3 characters
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 3;
        }

        // =========================================
        // Validate Password
        // =========================================
        public static bool IsValidPassword(string password)
        {
            // Password rules:
            // - Not empty
            // - At least 6 characters
            // - Must contain letters and numbers

            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 6)
                return false;

            bool hasLetter = Regex.IsMatch(password, "[a-zA-Z]");
            bool hasNumber = Regex.IsMatch(password, "[0-9]");

            return hasLetter && hasNumber;
        }

        // =========================================
        // Validate Movie Title
        // =========================================
        public static bool IsValidMovieTitle(string title)
        {
            // Title must not be empty
            return !string.IsNullOrWhiteSpace(title) && title.Length >= 2;
        }

        // =========================================
        // Validate Genre
        // =========================================
        public static bool IsValidGenre(string genre)
        {
            // Genre must not be empty
            return !string.IsNullOrWhiteSpace(genre);
        }

        // =========================================
        // Validate Year
        // =========================================
        public static bool IsValidYear(int year)
        {
            // Year must be realistic (1900 - current year)
            int currentYear = DateTime.Now.Year;

            return year >= 1900 && year <= currentYear;
        }

        // =========================================
        // Validate Rating Score
        // =========================================
        public static bool IsValidRating(int score)
        {
            // Rating must be between 1 and 5
            return score >= 1 && score <= 5;
        }

        // =========================================
        // Validate Movie ID
        // =========================================
        public static bool IsValidId(int id)
        {
            // ID must be positive
            return id > 0;
        }
    }
}