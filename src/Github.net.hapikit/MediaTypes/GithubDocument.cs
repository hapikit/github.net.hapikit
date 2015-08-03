using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hapikit.Links;
using Hapikit.Links.IANA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace GitHubWebPack.MediaTypes
{
    public class GithubDocument
    {
     
        private JObject _doc;
        private JArray _List;
        private Dictionary<string, JToken> _Properties;
        private Dictionary<string, ILink> _Links;
        public List<GithubDocument> Items { get; set; } 
        public GithubDocument(Stream document, LinkFactory linkFactory)
        {
            var sr = new StreamReader(document);
            var root = JToken.Load(new JsonTextReader(sr));

            _doc = root as JObject;
            if (_doc != null)
            {
                Load(linkFactory);
                if (_doc["items"] != null)
                {
                    LoadItems(linkFactory, _doc["items"] as JArray);
                }
            }
            else
            {
                _List = root as JArray;
                LoadItems(linkFactory, _List);
            }
        }

        private void LoadItems(LinkFactory linkFactory, JArray jArray)
        {
            Items = new List<GithubDocument>();
            foreach (JObject doc in jArray)
            {
                var childDoc = new GithubDocument(doc, linkFactory);
                if (childDoc.Properties.ContainsKey("url"))
                {
                    var itemUrl = new Uri((string) childDoc.Properties["url"]);
                    var itemLink = linkFactory.CreateLink<ItemLink>();
                    itemLink.Target = itemUrl;
                    childDoc.Links.Add(itemLink.Relation, itemLink);
                }
                Items.Add(childDoc);
            }
        }

        public GithubDocument(JObject document, LinkFactory linkFactory)
        {
            _doc = document;
            if (_doc != null)
            {
                Load(linkFactory);
            }
        }




        public void Load(LinkFactory linkFactory)
        {
            
                // get all properties that end in _url
                _Links = _doc.Properties()
                    .Where(p => p.Name.EndsWith("_url"))
                    .Select<JProperty, ILink>(p =>
                    {
                        var link = linkFactory.CreateLink("http://api.github.com/rels/" + p.Name.Replace("_url", ""));
                        link.Target = new Uri((string)p.Value);
                        return link;
                    })
                    .ToDictionary(k => k.Relation, v => v);

                _Properties = _doc.Properties()
                    .Where(p => !p.Name.EndsWith("_url"))
                    .ToDictionary(k => k.Name, v => v.Value);
            
        }


        public Dictionary<string, ILink> Links
        {
            get { return _Links; }
        }

        public T GetLink<T>() where T : Link
        {
            var relation = LinkHelper.GetLinkRelationTypeName<T>();
            return _Links[relation] as T;
        }

        public Dictionary<string, JToken> Properties
        {
            get { return _Properties; }
        }
    }
}
