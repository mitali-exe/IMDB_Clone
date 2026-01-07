using Microsoft.EntityFrameworkCore;
using MovieClone.Models;

namespace MovieClone.Services
{
    public class MovieService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _db;
        private const string ApiKey = "YOUR_API_KEY";

        public MovieService(HttpClient http, AppDbContext db)
        {
            _http = http;
            _db = db;
        }

        public async Task<List<Movie>> GetTrendingMoviesAsync(int page = 1)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<TmdbResponse>(
                    $"https://api.themoviedb.org/3/trending/movie/week?api_key={ApiKey}&page={page}");

                return response?.Results ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Movie>();
            }
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<Movie>();

            try
            {
                var response = await _http.GetFromJsonAsync<TmdbResponse>(
                    $"https://api.themoviedb.org/3/search/movie?api_key={ApiKey}&query={Uri.EscapeDataString(query)}&page={page}");

                return response?.Results ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search Error: {ex.Message}");
                return new List<Movie>();
            }
        }


        public async Task<bool> SaveToWatchlist(Movie movie)
        {
            var exists = await _db.Watchlist.AnyAsync(m => m.TmdbId == movie.TmdbId);

            if (!exists)
            {
                var movieToSave = new Movie
                {
                    TmdbId = movie.TmdbId,
                    Title = movie.Title,
                    Overview = movie.Overview,
                    PosterPath = movie.PosterPath,
                    VoteAverage = movie.VoteAverage
                };

                _db.Watchlist.Add(movieToSave);
                await _db.SaveChangesAsync();
                return true; 
            }

            return false; 
        }

        private static readonly Dictionary<int, string> GenreMap = new()
{
    { 28, "Action" }, { 12, "Adventure" }, { 16, "Animation" }, { 35, "Comedy" },
    { 80, "Crime" }, { 99, "Documentary" }, { 18, "Drama" }, { 10751, "Family" },
    { 14, "Fantasy" }, { 36, "History" }, { 27, "Horror" }, { 10402, "Music" },
    { 9648, "Mystery" }, { 10749, "Romance" }, { 878, "Sci-Fi" }, { 10770, "TV Movie" },
    { 53, "Thriller" }, { 10752, "War" }, { 37, "Western" }
};

        public List<string> GetGenreNames(List<int> ids)
        {
            return ids.Select(id => GenreMap.ContainsKey(id) ? GenreMap[id] : "Unknown").ToList();
        }


        public async Task<string?> GetMovieTrailerKeyAsync(int tmdbId)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<VideoResponse>(
                    $"https://api.themoviedb.org/3/movie/{tmdbId}/videos?api_key={ApiKey}");

                
                var trailer = response?.Results.FirstOrDefault(v =>
                    v.Site == "YouTube" && v.Type == "Trailer");

                return trailer?.Key;
            }
            catch
            {
                return null;
            }
        }

        public class VideoResponse { public List<VideoResult> Results { get; set; } = new(); }
        public class VideoResult
        {
            public string Key { get; set; } = "";
            public string Site { get; set; } = "";
            public string Type { get; set; } = "";
        }

        public async Task<List<CastMember>> GetMovieCastAsync(int tmdbId)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<CreditsResponse>(
                    $"https://api.themoviedb.org/3/movie/{tmdbId}/credits?api_key={ApiKey}");

                // Take the top 10 actors to keep the UI clean
                return response?.Cast.Take(10).ToList() ?? new List<CastMember>();
            }
            catch
            {
                return new List<CastMember>();
            }
        }


    }


}
