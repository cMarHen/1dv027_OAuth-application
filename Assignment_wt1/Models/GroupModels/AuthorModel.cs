using System.Text.Json.Serialization;

namespace Assignment_wt1.Models.GroupModels
{
    [Serializable]
    public class AuthorModel
    {
        private string avatarUrl;

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl
        {
            get => avatarUrl;
            set => avatarUrl = value.StartsWith("https://")
                ? value
                : "https://gitlab.lnu.se/" + value;
        }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
