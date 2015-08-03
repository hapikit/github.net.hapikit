using GitHubLib;
using GitHubWebPack.Links;
using GitHubWebPack.MediaTypes;

namespace GitHubWebPack.Messages
{
    public class HomeDocument
    {
        public UserLink CurrentUserLink { get; set; }
        public CodeSearchLink CodeSearchLink { get; set; }
        public EmailsLink EmailsLink { get; set; }
        public EmojisLink EmojisLink { get; set; }
        public GistsLink PublicGists { get; set; }
        public GistsLink StarredGists { get; set; }
        public AvatarLink AvatarLink { get; set; }


        internal static HomeDocument Parse(GithubDocument githubDocument)
        {
            var home = new HomeDocument();
            foreach (var link in githubDocument.Links)
            {
                switch (link.Key)
                {
                    case "code_search_url":
                        home.CodeSearchLink = (CodeSearchLink)link.Value;
                        break;
                }
            }
            return home;
        }
    }
}