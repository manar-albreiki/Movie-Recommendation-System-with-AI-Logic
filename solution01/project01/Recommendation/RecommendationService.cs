using System;
using System.Collections.Generic;
using System.Linq;
using MovieRecommendationSystem.Models;
using MovieRecommendationSystem.AI;

namespace MovieRecommendationSystem.Services.Recommendation
{
    public class RecommendationService
    {
        private readonly RecommendationEngine engine;

        public RecommendationService()
        {
            engine = new RecommendationEngine();
        }

        // =========================================
        // GET RECOMMENDATIONS (PERSONALIZED FIX)
        // =========================================
        public List<Movie> GetRecommendations(User user)
        {
            if (user == null)
                return new List<Movie>();

            var recommendations = engine.GenerateRecommendations(user);

            if (recommendations == null)
                return new List<Movie>();

            // 🔥 make results different per user
            return recommendations
                .OrderByDescending(m =>
                    user.WatchHistory != null &&
                    user.WatchHistory.Contains(m.Id) ? 2 : 0)
                .ThenByDescending(m => m.Rating)
                .ToList();
        }
    }
}