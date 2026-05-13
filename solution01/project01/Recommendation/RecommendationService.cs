using System;
using System.Collections.Generic;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.AI;

namespace MovieRecommendationSystem.Services.Recommendation
{
    public class RecommendationService
    {
        // =========================================
        // AI Engine Instance
        // =========================================
        private readonly RecommendationEngine engine;

        // =========================================
        // Constructor
        // =========================================
        public RecommendationService()
        {
            // Initialize AI Engine once
            engine = new RecommendationEngine();
        }

        // =========================================
        // Get AI-Powered Recommendations
        // =========================================
        public List<Movie> GetRecommendations(User user)
        {
            // =========================================
            // Validation
            // =========================================
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Invalid user.");
                Console.ResetColor();

                return new List<Movie>();
            }

            // =========================================
            // Get recommendations from AI Engine
            // =========================================
            List<Movie> recommendations = engine.GenerateRecommendations(user);

            // =========================================
            // Safety check
            // =========================================
            if (recommendations == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠ No recommendations found.");
                Console.ResetColor();

                return new List<Movie>();
            }

            // =========================================
            // Return final result
            // =========================================
            return recommendations;
        }
    }
}