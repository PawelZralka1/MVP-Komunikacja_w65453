using Projekcik1.Areas.Identity.Data;

namespace Projekcik1.Models
{
    public class UserGame
    {
        public int Id { get; set; }
        public string GameId { get; set; }  // IGDB Game ID
        public string Title { get; set; }
        public string CoverUrl { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
