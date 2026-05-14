using System;

namespace MovieRecommendationSystem.Models
{
    public class Rating
    {
        public int Id { get; set; }


        public int UserId { get; set; }

        public int MovieId { get; set; }

        public int Score { get; set; }

        public DateTime RatedAt { get; set; }

        public Rating()
        {
            RatedAt = DateTime.Now;
        }
    }
}