using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class ModDetails
    {
        [JsonProperty("iD")]
        public int Id { get; set; }

        [JsonProperty("gameId")]
        public int GameId { get; set; }

        [JsonProperty("gamePopularityRank")]
        public int GamePopularityRank { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("slug")]
        public string? Slug { get; set; }

        [JsonProperty("summary")]
        public string? Summary { get; set; }

        [JsonProperty("links")]
        public ModLinks? Links { get; set; }

        [JsonProperty("categories")]
        public List<Category>? Categories { get; set; }

        [JsonProperty("authors")]
        public List<Author>? Authors { get; set; }

        [JsonProperty("logo")]
        public Media? Logo { get; set; }

        [JsonProperty("screenshots")]
        public List<Media>? Screenshots { get; set; }

        [JsonProperty("videos")]
        public List<Media>? Videos { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("downloadCount")]
        public int DownloadCount { get; set; }

        [JsonProperty("isFeatured")]
        public bool IsFeatured { get; set; }

        [JsonProperty("classId")]
        public int ClassId { get; set; }

        [JsonProperty("primaryCategoryId")]
        public int PrimaryCategoryId { get; set; }
    }
}
