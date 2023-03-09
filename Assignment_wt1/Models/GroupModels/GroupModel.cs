using System.Text.Json.Serialization;

namespace Assignment_wt1.Models.GroupModels
{
    [Serializable]
    public class GroupModel
    {
        private string avatarUrl;
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("count")]
        public int ProjectCount { get; set; }

        [JsonPropertyName("webUrl")]
        public string WebUrl { get; set; }

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl
        {
            get => avatarUrl;
            set => avatarUrl = value.StartsWith("https://")
                ? value
                : "https://gitlab.lnu.se/" + value;
        }

        [JsonPropertyName("fullPath")]
        public string FullPath { get; set; }

        [JsonPropertyName("endCursor")]
        public string ProjectEndCursor { get; set; }

        [JsonPropertyName("projects")]
        public List<ProjectModel> Projects { get; set; }
    }
}
