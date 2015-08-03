using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GitHubLib;
using GitHubWebPack;
using GitHubWebPack.Links;
using GitHubWebPack.MediaTypes;
using Hapikit.Links;

namespace GitLinksConsole
{
    public class GithubClientState
    {
        private readonly LinkFactory _linkFactory;
        private GithubDocument _homeDocument;
        private UserLink.UserResult _currentUser;
        private GithubDocument _lastDocument;

        public GithubClientState(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
            ConfigureLinkBehaviour();
        }

        private void ConfigureLinkBehaviour()
        {
            //_linkFactory.SetHandler<HomeLink>(new ActionResponseHandler(HandleHomeLinkResponse));
            //_linkFactory.SetHandler<UserLink>(new ActionResponseHandler(HandleUserLinkResponse));
            //_linkFactory.SetHandler<CodeSearchLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            //_linkFactory.SetHandler<EmojisLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            //_linkFactory.SetHandler<FollowingLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            //_linkFactory.SetHandler<FollowersLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            //_linkFactory.SetHandler<GistsLink>(new ActionResponseHandler(HandleStandardDocumentResponse));
            //_linkFactory.SetHandler<ItemLink>(new ActionResponseHandler(HandleItemResponse));
        }

        public UserLink.UserResult CurrentUser
        {
            get { return _currentUser; }
        }

        public GithubDocument HomeDocument
        {
            get { return _homeDocument; }
        }

        public GithubDocument LastDocument
        {
            get { return _lastDocument; }
        }

        public List<GithubDocument> List
        {
            get { return _list; }
          
        }

        private async Task HandleHomeLinkResponse(string link, HttpResponseMessage responseMessage)
        {
            _homeDocument = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
            _lastDocument = HomeDocument;
        }

        private async Task HandleUserLinkResponse(string link, HttpResponseMessage responseMessage)
        {
            _lastDocument = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
            _currentUser = UserLink.InterpretMessageBody(_lastDocument);
        }

        private async Task HandleStandardDocumentResponse(string link, HttpResponseMessage responseMessage)
        {
            _lastDocument = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
        }

        private async Task HandleItemResponse(string link, HttpResponseMessage responseMessage)
        {
            var itemDoc = await responseMessage.Content.ReadAsGithubDocumentAsync(_linkFactory);
            List.Add(itemDoc);
        }

        internal void ClearList()
        {
            List.Clear();
        }

        private List<GithubDocument> _list = new List<GithubDocument>(); 
    }
}