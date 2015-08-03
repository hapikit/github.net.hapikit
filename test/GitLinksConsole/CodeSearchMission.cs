using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GitHubLib;
using GitHubWebPack.Links;
using GitHubWebPack.MediaTypes;
using Hapikit;
using Hapikit.Links;


namespace GitLinksConsole
{
    public class CodeSearchMission : IResponseHandler
    {
        private readonly HttpClient _httpClient;
        private readonly LinkFactory _linkFactory;
        private string _Query;
        private CodeSearchLink _CodeSearchLink;
        private string _FileNameFilter;
        public List<string> FileContents { get; set; }


        public CodeSearchMission(HttpClient httpClient, LinkFactory linkFactory)
        {
            _httpClient = httpClient;
            _linkFactory = linkFactory;
            FileContents = new List<string>();
        }

        public async Task FindAsync(string query, string fileNameFilter)
        {
            _Query = query;
            _FileNameFilter = fileNameFilter;

            var homelink = new HomeLink() { Target = new Uri("https://api.github.com") };
            
            // Apply representation back onto mission state machine
            await _httpClient.FollowLinkAsync(homelink)
                             .ApplyRepresentationToAsync(this);

        }

        public async Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            // Handle the Uniform Interface in one place: 300s,400s,500s

            // Parse the media type based on the response 
            GithubDocument githubDocument = null;
            if (IsGithubDocument(responseMessage.Content))
            {
                githubDocument = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);

            }

            // Dispatch on Link Type
            if (linkRelation == LinkHelper.GetLinkRelationTypeName<HomeLink>())
            {
                await HandleHomeDocument(githubDocument);
            }

            else if (linkRelation == LinkHelper.GetLinkRelationTypeName<CodeSearchLink>())
            {
                await HandleCodeSearchDocument(githubDocument);
            }

            else if (linkRelation == LinkHelper.GetLinkRelationTypeName<CodeFileLink>())
            {
                HandleCodeFileDocument(githubDocument);
            }

            return responseMessage;
        }

       

        private async Task HandleHomeDocument(GithubDocument githubDocument)
        {
            _CodeSearchLink = githubDocument.GetLink<CodeSearchLink>();
            _CodeSearchLink.Query = _Query;
            await _httpClient.FollowLinkAsync(_CodeSearchLink).ApplyRepresentationToAsync(this);
        }

        private async Task HandleCodeSearchDocument(GithubDocument githubDocument)
        {
            var searchResults = CodeSearchLink.InterpretMessageBody(githubDocument);
            var fileLinks = searchResults.Items
                .Where(i => i.Name.Contains(_FileNameFilter) && i.CodeFileLink != null)
                .Select(i => i.CodeFileLink);

            foreach (var fileLink in fileLinks)
            {
                await _httpClient.FollowLinkAsync(fileLink).ApplyRepresentationToAsync(this);
            }
        }

        private void HandleCodeFileDocument(GithubDocument githubDocument)
        {
            // Use Type Specific Link to interpret results
            var codeFile = CodeFileLink.InterpretMessageBody(githubDocument);
            FileContents.Add(codeFile.Content);
        }

        private bool IsGithubDocument(HttpContent httpContent)
        {
            return true;
        }
        
    }
}