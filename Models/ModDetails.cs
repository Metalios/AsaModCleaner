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

        [JsonProperty("links")]
        public ModLinks? Links { get; set; }

        [JsonProperty("summary")]
        public string? Summary { get; set; }

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

        [JsonProperty("categories")]
        public List<ModCategory>? Categories { get; set; }

        [JsonProperty("authors")]
        public List<Author>? Authors { get; set; }

        [JsonProperty("logo")]
        public ModLogo? Logo { get; set; }

        [JsonProperty("screenshots")]
        public List<object>? Screenshots { get; set; }

        [JsonProperty("videos")]
        public List<object>? Videos { get; set; }

        [JsonProperty("mainFileId")]
        public int MainFileId { get; set; }

        [JsonProperty("latestFiles")]
        public List<ModFile>? LatestFiles { get; set; }

        [JsonProperty("latestFilesIndexes")]
        public List<LatestFileIndex>? LatestFilesIndexes { get; set; }

        [JsonProperty("dateCreated")]
        public string? DateCreated { get; set; }

        [JsonProperty("dateModified")]
        public string? DateModified { get; set; }

        [JsonProperty("dateReleased")]
        public string? DateReleased { get; set; }

        [JsonProperty("allowModDistribution")]
        public bool AllowModDistribution { get; set; }

        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonProperty("ratingDetails")]
        public RatingDetails? RatingDetails { get; set; }

        [JsonProperty("premiumDetails")]
        public PremiumDetails? PremiumDetails { get; set; }
    }
}
