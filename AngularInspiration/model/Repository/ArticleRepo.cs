using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AngularInspiration.Model
{
    
    public abstract class ArticleRepo
    {

        protected async Task<string> GetRequestStringContent(string uri)
        {
            string content;
            using (var client = new HttpClient()){
                var response = await client.GetAsync(uri);
                content = await response.Content.ReadAsStringAsync();
            }   
            return content;
        }

    }
}

// DOAJ =  https://doaj.org/article/<article-id>
