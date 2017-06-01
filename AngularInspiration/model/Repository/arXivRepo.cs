using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AngularInspiration.Model
{
    
    public class arXivRepo : BaseRepo
    {
        public override async Task<InspirationCollection> Search(string matchPhrase, int num, int page = 0)
        {
            var result = await GetFeed(matchPhrase, num, page);
            var collection = new InspirationCollection();
            collection.SearchTerm = matchPhrase;
            collection.TotalMatching = result["opensearch:totalResults"]["#text"].Value<int>(); 
            foreach(var e in result["entry"])
            {
                var card = new InspirationCard();
                card.Summary = e["summary"].Value<string>();
                card.Title = e["title"].Value<string>();
                card.Link = e["id"].Value<string>();
                card.Id = e["id"].Value<string>();
                collection.Add(card);
            }
            return collection;
        } 
        
        private static string uri = "http://export.arxiv.org/api/query?search_query=all:{query}" + "&start={start}&max_results={maxResults}";

        private async Task<JObject> GetFeed(string query, int num, int page)
        {
            var finalUri = uri.Replace("{query}", query)
                .Replace("{maxResults}",num.ToString())
                .Replace("{start}",page.ToString()); // temp solution here
            
            var responseContent = await GetRequestStringContent(finalUri);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseContent);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return Clean(JObject.Parse(jsonText));

        }

        private JObject Clean(JObject root)
        {
            root.Property("?xml").Remove();
            root = JObject.FromObject(root["feed"]);
            var token = root["entry"];
            if(!( token is JArray) && token != null) // fixing deserialisation of xml where 1 entry gets returned without array
            {
                root.Property("entry").Remove();
                var jarray = new JArray();
                jarray.Add(token);
                root.Add("entry", jarray);
            }
            else if(token == null)
            {
                root.Add("entry", new JArray()); // add an empty array to represent zero results
            }
            return root;
        }


    }
}

// DOAJ =  https://doaj.org/article/<article-id>
