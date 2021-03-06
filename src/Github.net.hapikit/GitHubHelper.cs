﻿using GitHubLib;
using GitHubWebPack.Links;
using Hapikit.Links;


namespace GitHubWebPack
{
    public class GitHubHelper
    {
        public static void RegisterGitHubLinks(LinkFactory linkFactory)
        {
            linkFactory.AddLinkType<HomeLink>();
            linkFactory.AddLinkType<UserLink>();
            linkFactory.AddLinkType<CodeSearchLink>();
            linkFactory.AddLinkType<EmojisLink>();
            linkFactory.AddLinkType<FollowingLink>();
            linkFactory.AddLinkType<FollowersLink>();
            linkFactory.AddLinkType<GistsLink>();
        }
    }
}