using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngularInspiration.Model.Contract;
using Newtonsoft.Json;

namespace AngularInspiration.Model
{
    
    public abstract class BaseRepo : IInspirationRepository
    {
        public abstract Task<InspirationCollection> Search(string matchPhrase, int num, int page = 0);
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
