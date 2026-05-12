using MovieRecommendationSystem.Models;
using project01.Recommendation;
using System;
using System.Collections.Generic;

namespace MovieRecommendationSystem.Services.Recommendation
{
    public class RecommendationService
    {
        private ContentBasedService contentService;
        private CollaborativeFilteringService collaborativeService;

        // Constructor
        public RecommendationService(
            ContentBasedService content,
            CollaborativeFilteringService collaborative)
        {
            contentService = content;
            collaborativeService = collaborative;
        }

        // =========================================
        // Get Hybrid Recommendations
        // =========================================
        public List<Movie> GetRecommendations(User user)
        {
            // Get content-based recommendations
            List<Movie> contentBased = contentService.GetRecommendations(user);

            // Get collaborative filtering recommendations
            List<Movie> collaborative = collaborativeService.GetRecommendations(user);

            // Merge both lists (Hybrid system)
            List<Movie> finalList = new List<Movie>();

            finalList.AddRange(contentBased);

            foreach (var movie in collaborative)
            {
                if (!finalList.Exists(m => m.Id == movie.Id))
                    finalList.Add(movie);
            }

            return finalList;
        }
    }
}