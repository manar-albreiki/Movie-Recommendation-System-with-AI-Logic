// Movie.cs

using System;
using System.Collections.Generic;

namespace MovieRecommendationSystem.Models
{
    public class Movie
    {
        private double rating;

        public int Id { get; set; }

        public string Title { get; set; }

        public string Genre { get; set; }

        public string Description { get; set; }

        public int ReleaseYear { get; set; }

        public string Director { get; set; }

        public List<string> Cast { get; set; }

        public List<string> Tags { get; set; }

        public double Rating
        {
            get { return rating; }
            set
            {
                if (value >= 1 && value <= 5)
                    rating = value;
            }
        }

        public Movie()
        {
            Cast = new List<string>();
            Tags = new List<string>();
        }
    }
}