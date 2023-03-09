using Assignment_wt1.Interfaces;
using Assignment_wt1.Models.GroupModels;
using Assignment_wt1.Utils;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Assignment_wt1.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientService _httpClientService;
        private readonly ISessionHandler _sessionHandler;
        private readonly ILogger _logger;

        public GroupsService(IConfiguration config, IHttpClientService httpClientService, ISessionHandler sessionHandler, ILogger<GroupsService> logger)
        {
            _config = config;
            _httpClientService = httpClientService;
            _sessionHandler = sessionHandler;
            _logger = logger;
        }

        public async Task<CurrentUserModel> GetGitlabGroups(string token)
        {
            var queryProvider = new GraphQLQuery();

            var query = queryProvider.GetCurrentUserQuery();
            var currentUser = MapToCurrentUserModel(await FetchGraphQLData(token, query));

            return currentUser;
        }

        public async Task<CurrentUserModel> LoadMoreProjects(string token, CurrentUserModel user, string pathToGroup)
        {
            var queryProvider = new GraphQLQuery();
            // Latest endCursor of the selected project
            GroupModel group = user.Groups.Where(group => group.FullPath == pathToGroup).FirstOrDefault();
            var endCursor = group.ProjectEndCursor;

            // Fetch data
            var query = queryProvider.GetMoreProjectsQuery(pathToGroup, endCursor);
            var jsonElement = await FetchGraphQLData(token, query);

            // Create new ProjectModels
            var projects = jsonElement.GetProperty("group").GetProperty("projects");
            var projectModels = MapToProjectModels(projects.GetProperty("nodes").EnumerateArray());

            // Fill the current user and update endCursor
            group.Projects.AddRange(projectModels);
            group.ProjectEndCursor = projects.GetProperty("pageInfo").GetProperty("endCursor").ToString();

            return user;
        }

        public async Task<CurrentUserModel> LoadMoreGroups(string token, CurrentUserModel user)
        {
            var queryProvider = new GraphQLQuery();
            // Latest fetched group
            var endCursor = user.GroupEndCursor;

            // Fetch data
            var query = queryProvider.GetMoreGroupsQuery(endCursor);
            var jsonElement = await FetchGraphQLData(token, query);

            // Create new GroupModels
            var groups = jsonElement.GetProperty("currentUser").GetProperty("groupMemberships");
            var groupModels = MapToGroupModels(groups.GetProperty("nodes").EnumerateArray());

            // Fill the current user and update endCursor
            user.Groups.AddRange(groupModels);
            user.GroupEndCursor = groups.GetProperty("pageInfo").GetProperty("endCursor").ToString();

            return user;
        }

        private async Task<JsonElement> FetchGraphQLData(string token, string query)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Access token not present in session.");
            }

            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            };

            var response = await _httpClientService.PostGraphQLAsync(_config["GitlabLinks:GraphQL"], query, headers);

            var responseString = await response.Content.ReadAsStringAsync();

            var document = JsonDocument.Parse(responseString);

            return document.RootElement;
        }

        private CurrentUserModel MapToCurrentUserModel(JsonElement responseJson)
        {
            var currentUser = new CurrentUserModel
            {
                UserName = responseJson.GetProperty("currentUser").GetProperty("username").ToString(),
                GroupMemerships = responseJson.GetProperty("currentUser").GetProperty("groupCount").GetInt32(),
                GroupEndCursor = responseJson.GetProperty("currentUser").GetProperty("groupMemberships").GetProperty("pageInfo").GetProperty("endCursor").ToString()
            };

            var groupNodes = responseJson.GetProperty("currentUser").GetProperty("groupMemberships").GetProperty("nodes");

            if (groupNodes.ValueKind == JsonValueKind.Array)
            {
                currentUser.Groups = MapToGroupModels(groupNodes.EnumerateArray());
            }

            return currentUser;
        }

        private List<GroupModel> MapToGroupModels(JsonElement.ArrayEnumerator groupNodes)
        {
            var groupModels = new List<GroupModel>();

            foreach (var groupNode in groupNodes)
            {
                var group = new GroupModel
                {
                    Name = groupNode.GetProperty("group").GetProperty("name").ToString(),
                    WebUrl = groupNode.GetProperty("group").GetProperty("webUrl").ToString(),
                    AvatarUrl = groupNode.GetProperty("group").GetProperty("avatarUrl").ToString() ?? null,
                    FullPath = groupNode.GetProperty("group").GetProperty("fullPath").ToString(),
                    ProjectCount = groupNode.GetProperty("group").GetProperty("projects").GetProperty("count").GetInt32(),
                    ProjectEndCursor = groupNode.GetProperty("group").GetProperty("projects").GetProperty("pageInfo").GetProperty("endCursor").ToString(),
                    Projects = new List<ProjectModel>()
                };

                var projectNodes = groupNode.GetProperty("group").GetProperty("projects").GetProperty("nodes");

                if (projectNodes.ValueKind == JsonValueKind.Array)
                {
                    group.Projects = MapToProjectModels(projectNodes.EnumerateArray());
                }

                groupModels.Add(group);
            }

            return groupModels;
        }

        private List<ProjectModel> MapToProjectModels(JsonElement.ArrayEnumerator projectNodes)
        {
            var projectModels = new List<ProjectModel>();

            foreach (var projectNode in projectNodes)
            {
                var project = new ProjectModel
                {
                    Name = projectNode.GetProperty("name").ToString(),
                    WebUrl = projectNode.GetProperty("webUrl").ToString(),
                    AvatarUrl = projectNode.GetProperty("avatarUrl").ToString(),
                    FullPath = projectNode.GetProperty("fullPath").ToString(),
                    NameWithNamespace = projectNode.GetProperty("nameWithNamespace").ToString(),
                    LastActivityAt = DateTime.Parse(projectNode.GetProperty("lastActivityAt").ToString()),
                    AuthoredDate = DateTime.Parse(projectNode.GetProperty("repository").GetProperty("tree").GetProperty("lastCommit").GetProperty("authoredDate").ToString()),
                    Author = JsonSerializer.Deserialize<AuthorModel>(projectNode.GetProperty("repository").GetProperty("tree").GetProperty("lastCommit").GetProperty("author"))
                };

                projectModels.Add(project);
            }

            return projectModels;
        }
    }
}
