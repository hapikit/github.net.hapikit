using System;
using System.Text;
using GitHubLib;
using GitHubWebPack.MediaTypes;
using Hapikit.Links;


namespace GitHubWebPack.Links
{
    [LinkRelationType("http://api.github.com/rels/file")]
    public class CodeFileLink : Link
    {
        public static CodeFile InterpretMessageBody(GithubDocument document)
        {
            var bytes = Convert.FromBase64String((string)document.Properties["content"]);
            return new CodeFile
            {
                
                Content = Encoding.UTF8.GetString(bytes,0, bytes.Length)
            };
        }

        public class CodeFile
        {
            public string Content { get; set; }            
        }
    }
}