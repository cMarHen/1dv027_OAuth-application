namespace Assignment_wt1.Utils
{
    /// <summary>
    /// Class that provides query strings used in Gitlab GraphQL API
    /// </summary>
    public class GraphQLQuery
    {
        private readonly string _query = @"
        query {
            currentUser {
                username
                groupCount
                groupMemberships(first: $groupCount) {
                    pageInfo {
                        hasNextPage
                        endCursor
                    }
                    nodes {
                        group {
                            name
                            webUrl
                            avatarUrl
                            fullPath
                            projects(first: $projectCount, includeSubgroups: true) {
                                pageInfo {
                                    hasNextPage
                                    endCursor
                                }
                                count
                                nodes {
                                    name
                                    webUrl
                                    avatarUrl
                                    fullPath
                                    nameWithNamespace
                                    lastActivityAt
                                    repository {
                                        tree {
                                            lastCommit {
                                                authoredDate
                                                author {
                                                    name
                                                    avatarUrl
                                                    username
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }";

        /// <summary>
        /// To get a predefined query from "currentUser".
        /// </summary>
        /// <param name="groupCount">Amount of groups, standard = 3</param>
        /// <param name="projectCount">Amount of projects for each group, standard = 5</param>
        /// <returns></returns>
        public string GetCurrentUserQuery(int groupCount = 3, int projectCount = 5)
        {
            var query = _query
                .Replace("$groupCount", groupCount.ToString())
                .Replace("$projectCount", projectCount.ToString());

            return query;
        }

        /// <summary>
        /// To get a predefined query to load more groups from a specified endCursor.
        /// </summary>
        /// <param name="endCursor">String endCursor where to start looking for groups</param>
        /// <param name="groupCount">Amount of groups, standard = 3</param>
        /// <param name="projectCount">Amount of projects for each group, standard = 5</param>
        /// <returns></returns>
        public string GetMoreGroupsQuery(string endCursor, int groupCount = 3, int projectCount = 5)
        {
            var query = @"
            query {
                currentUser {
                    groupCount
                    groupMemberships(first: $groupCount, after: ""$endCursor"") {
                        pageInfo {
                            hasNextPage
                            endCursor
                        }
                        nodes {
                            group {
                                name
                                webUrl
                                avatarUrl
                                fullPath
                                projects(first: $projectCount, includeSubgroups: true) {
                                    pageInfo {
                                        hasNextPage
                                        endCursor
                                    }
                                    count
                                    nodes {
                                        name
                                        webUrl
                                        avatarUrl
                                        fullPath
                                        nameWithNamespace
                                        lastActivityAt
                                        repository {
                                            tree {
                                                lastCommit {
                                                    authoredDate
                                                    author {
                                                        name
                                                        avatarUrl
                                                        username
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }"
                .Replace("$groupCount", groupCount.ToString())
                .Replace("$projectCount", projectCount.ToString())
                .Replace("$endCursor", endCursor);

            return query;
        }

        /// <summary>
        /// To get a predefined query to load more projects from a specified group and endCursor.
        /// </summary>
        /// <param name="groupPath">The relative path to the group to load projects from.</param>
        /// <param name="endCursor">String endCursor where to start looking for projects</param>
        /// <param name="projectCount">Amount of projects from the group, standard = 5</param>
        /// <returns></returns>
        public string GetMoreProjectsQuery(string groupPath, string endCursor, int projectCount = 5)
        {
            var query = @"
            query {
                group(fullPath: ""$groupPath"") {
                    webUrl
                    avatarUrl
                    fullPath
                    projects(first: $projectCount, after: ""$endCursor"") {
                    	pageInfo {
                          hasNextPage
                          endCursor
                        }
                        count
                        nodes {
                            name
                            webUrl
                            avatarUrl
                            fullPath
                            nameWithNamespace
                            lastActivityAt
                            repository {
                                tree {
                                    lastCommit {
                                        authoredDate
                                        author {
                                            name
                                            avatarUrl
                                            username
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }"
                .Replace("$groupPath", groupPath)
                .Replace("$projectCount", projectCount.ToString())
                .Replace("$endCursor", endCursor);

            return query;
        }
    }
}
