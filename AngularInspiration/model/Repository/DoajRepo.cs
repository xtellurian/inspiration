using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AngularInspiration.Model
{
    
    public class DoajRepo : BaseRepo
    {
        private string _doajBase =  @"https://doaj.org/article/{article-id}";
        private const string uri = "https://doaj.org/api/v1/search/articles/{query}?page={start}&pageSize={maxResults}";

        public override async Task<InspirationCollection> Search(string matchPhrase, int num, int page = 0)
        {
            var result = await GetFeed(matchPhrase, num, page);
            var collection = new InspirationCollection();
            collection.SearchTerm = matchPhrase;
            collection.TotalMatching = result["total"].Value<int>();
            foreach(var item in result["results"]){
                var card = new InspirationCard();
                card.Title = item["bibjson"]["title"].Value<string>();
                card.Summary = item["bibjson"]["abstract"]?.Value<string>();
                card.Link = _doajBase.Replace("{article-id}",item["id"].Value<string>());
                card.Id = item["id"].Value<string>();
                collection.Add(card);
            }
            return collection;

        }

        private async Task<JObject> GetFeed(string query, int num, int page)
        {
            var finalUri = uri.Replace("{query}", query)
                .Replace("{maxResults}",num.ToString())
                .Replace("{start}",page.ToString()); // temp solution here
            var stringContent = await GetRequestStringContent(finalUri);

            return JObject.Parse(stringContent);
        }


    }
}

// DOAJ =  https://doaj.org/article/<article-id>
