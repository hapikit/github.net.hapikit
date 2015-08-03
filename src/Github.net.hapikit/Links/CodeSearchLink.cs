using System;
using System.Collections.Generic;
using System.Linq;
using GitHubWebPack.MediaTypes;
using Hapikit.Links;
using Hapikit.RequestBuilders;

namespace GitHubWebPack.Links
{
    [LinkRelationType("http://api.github.com/rels/code_search")]
    public class CodeSearchLink : Link
    {
        public enum SearchSort
        {
            defaultsort,
            stars,
            forks,
            updated
        };

        [LinkParameter("query")]
        public string Query { get; set; }
        
        [LinkParameter("page",Default=0)]
        public int Page { get; set; }

        [LinkParameter("per_page", Default = 0)]
        public int PerPage { get; set; }

        [LinkParameter("sort_order", Default = SearchSort.defaultsort)]
        public SearchSort Sort { get; set; }


        
        public static CodeSearchResults InterpretMessageBody(GithubDocument document)
        {
            return CodeSearchResults.Parse(document);
        }

        public class CodeSearchResults
        {
            public int Count { get; set; }
            public List<CodeSearchResult> Items { get; set; }

            public static CodeSearchResults Parse(GithubDocument document)
            {
                return new CodeSearchResults
                {
                    Count = (int) document.Properties["total_count"],
                    Items = document.Items.Select(CodeSearchResult.Parse).ToList()
                };
            }
        }
        public class CodeSearchResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Sha { get; set; }
            public CodeFileLink CodeFileLink { get; set; }

            public static CodeSearchResult Parse(GithubDocument document)
            {
                var result = new CodeSearchResult();
                foreach (var property in document.Properties)
                {
                    switch (property.Key)
                    {
                        case "name":
                            result.Name = (string)property.Value;
                            break;
                        case "path":
                            result.Path = (string)property.Value;
                            break;
                        case "sha":
                            result.Sha = (string)property.Value;
                            break;
                        case "url":
                            result.CodeFileLink = new CodeFileLink() {
                                Target = new Uri((string)property.Value)
                            };
                            break;
                    }
                }
                return result;
            }
        }
    }
}