using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GitHubLib;
using GitHubWebPack;
using GitHubWebPack.Links;
using Hapikit;
using Hapikit.Links;
using Hapikit.Templates;


namespace GitLinksConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //CodeSearchUsingLink().Wait();
            //TypedLinksFromGenericType().Wait();
            MissionizeCodeSearch().Wait();

            Console.ReadLine();
        }

        private static async Task CodeSearchUsingLink()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
                .UserAgent.Add(new ProductInfoHeaderValue("DarrelsGitClient", "1.0"));

            // API Specific type acts as Request Factory
            var codeSearchLink = new CodeSearchLink()
            {
                Template = new UriTemplate("https://api.github.com/search/code?q={query}{&page,per_page,sort_order}"),
                Query = ".Result user:darrelmiller"
            };

            var requestMessage = codeSearchLink.CreateRequest();
           
            var responseMessage = await httpClient.SendAsync(requestMessage);
            
            // ReadAs extension takes advantage of media type defined by API
            var githubdoc = await responseMessage.Content.ReadAsGithubDocumentAsync();

            // API Specific Link Type knows how to interpret generic media type
            var searchResults = CodeSearchLink.InterpretMessageBody(githubdoc);

            foreach (var result in searchResults.Items)
            {
                Console.WriteLine(result.Path);
            }

            // Harvest strongly typed links from strongly typed searchresults
            var fileLinks = searchResults.Items
                .Where(i => i.Name.Contains("Slack") && i.CodeFileLink != null)
                .Select(i => i.CodeFileLink);

            foreach (var fileLink in fileLinks)
            {
                // Following Links is a way of navigating the API's object graph
                var fileResponse = await httpClient.FollowLinkAsync(fileLink);

                // Reuse media type parser
                var fileDoc = await fileResponse.Content.ReadAsGithubDocumentAsync();

                // Use Type Specific Link to interpret results
                var codeFile = CodeFileLink.InterpretMessageBody(fileDoc);
                Console.WriteLine(codeFile.Content);
            }
        }  


       

        private static async Task TypedLinksFromGenericType()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
                .UserAgent.Add(new ProductInfoHeaderValue("DarrelsGitClient", "1.0"));

            // Factory for typed links
            var linkFactory = new LinkFactory();
            GitHubHelper.RegisterGitHubLinks(linkFactory);

            // Get Home Document, without strongly typed object
            var homelink = new HomeLink();
            var homeResponse = await httpClient.FollowLinkAsync(homelink);
            var githubDocument = await homeResponse.Content.ReadAsGithubDocumentAsync(linkFactory);

            // Retrieve specific link from generic document
            var codeSearchLink = githubDocument.GetLink<CodeSearchLink>();
            codeSearchLink.Query = ".Result user:darrelmiller";

            var responseMessage = await httpClient.FollowLinkAsync(codeSearchLink);
            var githubDocument2 = await responseMessage.Content.ReadAsGithubDocumentAsync();
            var searchResults = CodeSearchLink.InterpretMessageBody(githubDocument2);

            // Show Results
            foreach (var result in searchResults.Items)
            {
                Console.WriteLine(result.Path);
            }
        }

        private static async Task MissionizeCodeSearch()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
                .UserAgent.Add(new ProductInfoHeaderValue("DarrelsGitClient", "1.0"));
            var linkFactory = new LinkFactory();
            GitHubHelper.RegisterGitHubLinks(linkFactory);


            var codeSearchMission = new CodeSearchMission(httpClient,linkFactory);

            await codeSearchMission.FindAsync("Result user:darrelmiller", "Slack");

    
            foreach (var result in codeSearchMission.FileContents)
            {
                Console.WriteLine(result);
            }
        }
    }
}

