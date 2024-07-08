using Newtonsoft.Json;
using Projekcik1.Models;

namespace Projekcik1.Services
{
    public class IGDBService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IGDBService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Game>> GetGamesAsync()
        {
            var clientId = _configuration["IGDB:ClientId"];
            var accessToken = _configuration["IGDB:AccessToken"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            Console.WriteLine($"Sending request to IGDB API...");
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games",
                new StringContent("fields name,summary,rating,cover.url; limit 50;"));

            Console.WriteLine($"Response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response content: {content}");
                var games = JsonConvert.DeserializeObject<List<Game>>(content);
                return games;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error content: {errorContent}");
            }

            return new List<Game>();
        }

        public async Task<Game> GetGameDetailsAsync(long id)
        {
            var clientId = _configuration["IGDB:ClientId"];
            var accessToken = _configuration["IGDB:AccessToken"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var query = $"fields name,summary,rating,cover.url,genres.name,platforms.name,first_release_date; where id = {id};";
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games",
                new StringContent(query));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var games = JsonConvert.DeserializeObject<List<Game>>(content);
                return games.FirstOrDefault();
            }

            return null;
        }

        public async Task<PaginatedList<Game>> SearchGamesAsync(string searchString, int pageIndex, int pageSize)
        {
            var clientId = _configuration["IGDB:ClientId"];
            var accessToken = _configuration["IGDB:AccessToken"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var query = $"fields name,summary,rating,cover.url; limit {pageSize}; offset {(pageIndex - 1) * pageSize};";
            if (!string.IsNullOrEmpty(searchString))
            {
                query += $" search \"{searchString}\";";
            }
            else
            {
                query += " sort rating desc;";
            }

            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games",
                new StringContent(query));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var games = JsonConvert.DeserializeObject<List<Game>>(content);

                // Get total count (this requires a separate API call)
                var countQuery = $"fields id; {(string.IsNullOrEmpty(searchString) ? "" : $"search \"{searchString}\";")} limit 1;";
                var countResponse = await _httpClient.PostAsync("https://api.igdb.com/v4/games/count",
                    new StringContent(countQuery));
                var countContent = await countResponse.Content.ReadAsStringAsync();
                var countObject = JsonConvert.DeserializeObject<dynamic>(countContent);
                var totalCount = (int)countObject.count;

                return new PaginatedList<Game>(games, totalCount, pageIndex, pageSize);
            }

            return new PaginatedList<Game>(new List<Game>(), 0, pageIndex, pageSize);
        }
    }
}
