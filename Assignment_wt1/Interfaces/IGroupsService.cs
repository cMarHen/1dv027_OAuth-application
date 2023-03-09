using Assignment_wt1.Models;
using Assignment_wt1.Models.GroupModels;

namespace Assignment_wt1.Interfaces
{
    public interface IGroupsService
    {
        public Task<CurrentUserModel> GetGitlabGroups(string token);
        public Task<CurrentUserModel> LoadMoreProjects(string token, CurrentUserModel user, string pathToGroup);
        public Task<CurrentUserModel> LoadMoreGroups(string token, CurrentUserModel user);
    }
}
