﻿namespace Assignment_wt1.Models
{
    public class GitlabAccessData
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public string? id_token { get; set; }
        public int expires_in { get; set; }
        public string? refresh_token { get; set; }
        public int created_at { get; set; }
    }
}
