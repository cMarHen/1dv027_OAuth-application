using System.Text.Json.Serialization;

namespace Assignment_wt1.Models.GroupModels
{
    [Serializable]
    public class ProjectModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("web_url")]
        public string WebUrl { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("path_with_namespace")]
        public string FullPath { get; set; }

        [JsonPropertyName("name_with_namespace")]
        public string NameWithNamespace { get; set; }

        [JsonPropertyName("last_activity_at")]
        public DateTime LastActivityAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime AuthoredDate { get; set; }

        [JsonPropertyName("author")]
        public AuthorModel Author { get; set; }
    }
}
