using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace MovieClone.Models
{
    public class Movie
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }

        [JsonPropertyName("id")] 
        public int TmdbId { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int> GenreIds { get; set; } = new();

        public string FullPosterUrl => !string.IsNullOrEmpty(PosterPath)
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
            : "https://via.placeholder.com/500x750?text=No+Image";
    }

    public class TmdbResponse
    {
        [JsonPropertyName("results")]
        public List<Movie> Results { get; set; } = new();
    }

    public class CastMember
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("character")]
        public string Character { get; set; } = "";

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        public string ProfileImageUrl => !string.IsNullOrEmpty(ProfilePath)
            ? $"https://image.tmdb.org/t/p/w185{ProfilePath}"
            : "https://via.placeholder.com/185x278?text=No+Image";
    }

    public class CreditsResponse
    {
        [JsonPropertyName("cast")]
        public List<CastMember> Cast { get; set; } = new();
    }
}