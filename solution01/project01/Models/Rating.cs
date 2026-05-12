

using System;

namespace MovieRecommendationSystem.Models
{
    public class Rating
    {
        private int score;

        public int Id { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        public int Score
        {
            get { return score; }
            set
            {
                if (value >= 1 && value <= 5)
                    score = value;
            }
        }

        public DateTime RatedAt { get; set; }

        public Rating()
        {
            RatedAt = DateTime.Now;
        }
    }
}