namespace MovieRecommendationSystem.Models
{
    public class Admin : User
    {
        public string AdminRole { get; set; }

        public bool CanManageMovies { get; set; }

        public bool CanManageUsers { get; set; }

        public Admin()
        {
            CanManageMovies = true;
            CanManageUsers = true;
        }
    }
}