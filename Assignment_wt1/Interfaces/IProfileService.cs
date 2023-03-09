using Assignment_wt1.Models;

namespace Assignment_wt1.Interfaces
{
    public interface IProfileService
    {
        public Task<GitlabProfile> GetGitlabProfile(string token);
    }
}
