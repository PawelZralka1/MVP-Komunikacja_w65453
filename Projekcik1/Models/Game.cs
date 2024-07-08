using Newtonsoft.Json;

namespace Projekcik1.Models
{
    public class Game
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public double? Rating { get; set; }
        [JsonProperty("cover")]
        public Cover Cover { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Platform> Platforms { get; set; }
        [JsonProperty("first_release_date")]
        public long FirstReleaseDateUnix { get; set; }

        [JsonIgnore]
        public DateTime FirstReleaseDate => DateTimeOffset.FromUnixTimeSeconds(FirstReleaseDateUnix).DateTime;
    }

    public class Cover
    {
        public long Id { get; set; }
        public string Url { get; set; }
    }

    public class Genre
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class Platform
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
