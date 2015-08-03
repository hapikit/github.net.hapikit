using System;
using GitHubWebPack.MediaTypes;
using GitHubWebPack.Messages;
using Hapikit.Links;


namespace GitHubWebPack.Links
{
    [LinkRelationType("http://api.github.com/rels/home")]
    public class HomeLink : Link
    {
        public HomeLink() 
        {
            Target = new Uri("https://api.github.com"); 
        }
        public HomeDocument InterpretMessageBody(GithubDocument githubDocument)
        {
            return HomeDocument.Parse(githubDocument);
        }
    }
}