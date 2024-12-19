using Newtonsoft.Json;

namespace AsaModCleaner.Models
{
    public class ModFile
    {
        [JsonProperty("iD")]
        public int Id { get; set; }

        [JsonProperty("gameId")]
        public int GameId { get; set; }

        [JsonProperty("modId")]
        public int ModId { get; set; }

        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }

        [JsonProperty("filename")]
        public string? Filename { get; set; }

        [JsonProperty("releaseType")]
        public string? ReleaseType { get; set; }

        [JsonProperty("fileStatus")]
        public string? FileStatus { get; set; }

        [JsonProperty("hashes")]
        public List<FileHash>? Hashes { get; set; }

        [JsonProperty("fileDate")]
        public string? FileDate { get; set; }

        [JsonProperty("fileLength")]
        public long FileLength { get; set; }

        [JsonProperty("fileSizeOnDisk")]
        public long FileSizeOnDisk { get; set; }

        [JsonProperty("downloadCount")]
        public int DownloadCount { get; set; }

        [JsonProperty("downloadUrl")]
        public string? DownloadUrl { get; set; }

        [JsonProperty("gameVersions")]
        public List<string>? GameVersions { get; set; }

        [JsonProperty("sortableGameVersions")]
        public List<SortableGameVersion>? SortableGameVersions { get; set; }

        [JsonProperty("dependencies")]
        public List<object>? Dependencies { get; set; }

        [JsonProperty("exposeAsAlternative")]
        public bool ExposeAsAlternative { get; set; }

        [JsonProperty("parentProjectFileId")]
        public int ParentProjectFileId { get; set; }

        [JsonProperty("alternateFileId")]
        public int AlternateFileId { get; set; }

        [JsonProperty("isServerPack")]
        public bool IsServerPack { get; set; }

        [JsonProperty("serverPackFileId")]
        public int ServerPackFileId { get; set; }

        [JsonProperty("fileFingerprint")]
        public uint FileFingerprint { get; set; }

        [JsonProperty("modules")]
        public List<FileModule>? Modules { get; set; }
    }
}
