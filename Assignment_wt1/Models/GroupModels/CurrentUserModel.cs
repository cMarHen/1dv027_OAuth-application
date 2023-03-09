using System.Text.Json.Serialization;

namespace Assignment_wt1.Models.GroupModels
{
    [Serializable]
    public class CurrentUserModel
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
        [JsonPropertyName("groupCount")]
        public int GroupMemerships { get; set; }
        [JsonPropertyName("endCursor")]
        public string GroupEndCursor { get; set; }
        [JsonPropertyName("group")]
        public List<GroupModel> Groups { get; set; }
    }
}
