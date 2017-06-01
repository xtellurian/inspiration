using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace AngularInspiration.Model
{
    
    public class arXivRepo : ArticleRepo
    {
        public async Task<IEnumerable<IInspiration>> Search(string matchPhrase, int num)
        {
            var result = await GetFeed(matchPhrase, num);
            foreach(var i in result)
            {
                Console.WriteLine(i);
            }
            return null;
        } 
        
        private static string uri = "http://export.arxiv.org/api/query?search_query=all:{query}" + "&start={start}&max_results={maxResults}";

        private async Task<dynamic> GetFeed(string query, int num)
        {
            var finalUri = uri.Replace("{query}", query)
                .Replace("{maxResults}",num.ToString())
                .Replace("{start}","0"); // temp solution here
            
            var responseContent = await GetRequestStringContent(finalUri);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseContent);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return JsonConvert.DeserializeObject(jsonText);

        }


    }
}

// DOAJ =  https://doaj.org/article/<article-id>
