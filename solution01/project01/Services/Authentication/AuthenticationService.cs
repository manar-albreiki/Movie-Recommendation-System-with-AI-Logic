using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;

namespace MovieRecommendationSystem.Services.Authentication
{
    public class AuthenticationService
    {
        // List to store all users in the system
        private List<User> users;

        // Constructor
        // Used to initialize the users list
        public AuthenticationService(List<User> usersList)
        {
            users = usersList;
        }

        // ================================
        // Register Method
        // ================================
        // This method creates a new user account
        public bool Register(string username, string password)
        {
            // Check if username already exists
            bool userExists = users.Any(u => u.Username == username);

            if (userExists)
            {
                Console.WriteLine("Username already exists.");
                return false;
            }

            // Create new user object
            User newUser = new User
            {
                Id = users.Count + 1,
                Username = username,
                Password = password
            };

            // Add user to users list
            users.Add(newUser);

            Console.WriteLine("Registration completed successfully.");

            return true;
        }

        // ================================
        // Login Method
        // ================================
        // This method checks user credentials
        public User Login(string username, string password)
        {
            // Search for matching username and password
            User loggedUser = users.FirstOrDefault
            (
                u => u.Username == username &&
                     u.Password == password
            );

            // Check if user exists
            if (loggedUser != null)
            {
                Console.WriteLine("Login successful.");
                return loggedUser;
            }

            Console.WriteLine("Invalid username or password.");
            return null;
        }

        // ================================
        // Logout Method
        // ================================
        // This method logs out the user
        public void Logout(User user)
        {
            if (user != null)
            {
                Console.WriteLine($"{user.Username} logged out successfully.");
            }
        }
    }
}