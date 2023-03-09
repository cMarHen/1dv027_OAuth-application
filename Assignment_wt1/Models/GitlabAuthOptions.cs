namespace Assignment_wt1.Models
{
    public class GitlabAuthOptions
    {
        public string? ApplicationID { get; set; }
        public string? RedirectUri { get; set; }
        public string? ReqScopes { get; set; }
        public string? AuthorizationUri { get; set; }
        public string? State { get; set; }
        public string? CodeChallenge { get; set; }
        public string? CodeChallengeMethod { get; set; }

        public override string ToString()
        {
            return $"{AuthorizationUri}?client_id={ApplicationID}&redirect_uri={RedirectUri}&response_type=code&state={State}&scope={ReqScopes}&code_challenge={CodeChallenge}&code_challenge_method={CodeChallengeMethod}";
        }
    }
}
