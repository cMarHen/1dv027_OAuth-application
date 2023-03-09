using Assignment_wt1.Models;

namespace Assignment_wt1.Interfaces
{
    public interface IActivitiesService
    {
        public Task<List<GitlabActivities>> GetGitlabActivities(int desiredResults);
    }
}
