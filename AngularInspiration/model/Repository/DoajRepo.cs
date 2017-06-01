using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace AngularInspiration.Model
{
    
    public class DoajRepo : ArticleRepo
    {

        private const string uri = "https://doaj.org/api/v1/search/articles/{query}?page={start}&pageSize={maxResults}";

        private async Task<dynamic> GetFeed(string query, int num)
        {
            var finalUri = uri.Replace("{query}", query)
                .Replace("{maxResults}",num.ToString())
                .Replace("{start}","0"); // temp solution here
            var stringContent = await GetRequestStringContent(finalUri);

            return stringContent;
        }


    }
}

// DOAJ =  https://doaj.org/article/<article-id>
