

using project01.Models;
using System;
using System.Collections.Generic;

namespace MovieRecommendationSystem.Models
{
    public class User
    {
        private string username;
        private string password;

        public int Id { get; set; }

        public string Username
        {
            get { return username; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    username = value;
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && value.Length >= 6)
                    password = value;
            }
        }

        public List<string> FavoriteGenres { get; set; }

        public List<int> WatchHistory { get; set; }

        public List<Rating> Ratings { get; set; }

        public User()
        {
            FavoriteGenres = new List<string>();
            WatchHistory = new List<int>();
            Ratings = new List<Rating>();
        }
    }
}