using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngularInspiration.Model.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AngularInspiration.Model
{
    public class TextAnalyticsService : ITextAnalyticsService
    {
        private string _baseUrl;
        private string _apiKey;
        public TextAnalyticsService(string apiKey, string baseUrl = @"https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/")
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        public async Task<Dictionary<string, List<string>>> KeyPhrases<T>(
            IEnumerable<T> inputTexts, Func<T, string> getId, Func<T, string> getText)
        {
            var request = new TextRequest();

            foreach(var textObject in inputTexts)
            {
                var id = getId(textObject);
                var text = getText(textObject);
                request.Documents.Add(new TextDocument(id, text));
            }
            var response = await Get(request);
            return response;
        }

        private async Task<Dictionary<string, List<string>>> Get(TextRequest request)
        {
            JObject body;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseUrl}keyPhrases", content);
                body = JObject.Parse(await response.Content.ReadAsStringAsync());
            }
            
            var documents = body["documents"].Children().ToList();
            var result = new Dictionary<string, List<string>>();
            foreach(var doc in documents)
            {
                var id = doc["id"].Value<string>();
                result[id] = new List<string>();
                foreach(var s in doc["keyPhrases"])
                {
                    result[id].Add(s.Value<string>());
                }
            }
            return result;
        }

        private class TextRequest
        {
            public TextRequest()
            {
                Documents = new List<TextDocument>();
            }

            [JsonProperty("documents")]
            public List<TextDocument> Documents { get; set; }
        }
        
        private class TextDocument
        {
            public TextDocument(string id, string text, string language = "en")
            {
                Id = id;
                Language = language;
                Text = text;
            }

            [JsonProperty("language")]
            public string Language { get; private set; }

            [JsonProperty("id")]
            public string Id { get; private set; }

            [JsonProperty("text")]
            public string Text { get; private set; }
        } 
    }
}


