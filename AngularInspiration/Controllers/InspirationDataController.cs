using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngularInspiration.Model;
using AngularInspiration.Model.Contract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AngularInspiration.Controllers
{
    [Route("api/[controller]")]
    public class InspirationDataController : Controller
    {
        private IInspirationService _inspirationService;
        public InspirationDataController(IInspirationService inspirationService)
        {
            _inspirationService = inspirationService;
        }
        private string uri = @"https://prod-01.australiasoutheast.logic.azure.com:443/workflows/b3c5ce8f8eec4389a50fcd56e83ef36d/triggers/request/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Frequest%2Frun&sv=1.0&sig=q1qiGY7J3_sznpqMw4x1rLhoxEbbJOCbTpKhkL-phGc";

        private static InspirationRequest _lastRequest;
        private static dynamic _lastResponse;

        [HttpGet("[action]")]
        public async Task<IActionResult> Random()
        {
            // await Task.Delay(1000);
            // return Ok(EmptySummary);

            var request = MakeRequest();
            if(!request.IsValid()) return BadRequest("Invalid Data Request");
            Console.WriteLine($"last req: {_lastRequest}");
            if(!request.Equals( _lastRequest) || _lastResponse == null)
            {
                _lastResponse = await GetResponse(request);
            }
            _lastRequest = request;
            var doajArticles = GetDoajArticles();
            var arXivArticles = GetArXivArticles();
            var allArticles = arXivArticles.Concat(doajArticles).ToList();
            var len = allArticles.Count;
            if(len == 0) return Ok(EmptySummary);
            var rnd = new Random();
            return Ok(allArticles[rnd.Next(0,len)]);
        }
        private static InspirationSession _session;
        [HttpGet("[action]")]
        public async Task<IActionResult> NextRandom()
        {
            if(_session == null) _session = _inspirationService.NewSession();

            var request = MakeRequest();
            if(!request.IsValid()) return BadRequest("Invalid Data Request");
            var response = await _session.NextRandom(request.Queries.FirstOrDefault(), request.Num);
            return Ok(response);
        }

        private InspirationCard EmptySummary => new InspirationCard()
        {
            Title = "Whoops!",
            Summary = "Nothing matched your search"
        };

        private List<InspirationCard> GetArXivArticles()
        {
            var summaries = new List<InspirationCard>();
            foreach(var feed in _lastResponse.arXiv.feeds)
            {
                if(feed.feed.entry is JArray)
                {
                    foreach(var entry in feed.feed.entry)
                    {
                        try
                        {
                            var summary = new InspirationCard();
                            summary.Title = entry.title;
                            summary.Summary = entry.summary;
                            summary.Link = entry.id;
                            summaries.Add(summary);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(entry);
                            Console.WriteLine(ex);
                        }
                    }
                }
                else if (feed.feed.entry is JToken)
                {
                    var entry = feed.feed.entry;
                    try
                        {
                            var summary = new InspirationCard();
                            summary.Title = entry.title;
                            summary.Summary = entry.summary;
                            summary.Link = entry.id;
                            summaries.Add(summary);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(entry);
                            Console.WriteLine(ex);
                        }
                }
                
            }
            return summaries;
        }

        private string doajBase = "https://doaj.org/article/";
        private List<InspirationCard> GetDoajArticles()
        {
            var summaries = new List<InspirationCard>();
            foreach(var feed in _lastResponse.DOAJ.feeds)
            {
                foreach(var result in feed.results)
                {
                    var summary = new InspirationCard();
                    try
                    {
                        summary.Title = result.bibjson.title;
                        summary.Summary = result.bibjson["abstract"];
                        summary.Link = doajBase + result.id;
                        summaries.Add(summary);
                    }
                   catch(Exception ex)
                   {
                       Console.WriteLine(result);
                       Console.WriteLine(ex);
                       
                   }
                }
                
            }
            return summaries;
        }

        private InspirationRequest MakeRequest()
        {
            string query = HttpContext.Request.Query
                .FirstOrDefault(q => string.Compare(q.Key, "query", true) == 0)
                .Value;

            int num = 5; // default value
            int.TryParse(HttpContext.Request.Query
                .FirstOrDefault(q => string.Compare(q.Key, "num", true) == 0)
                .Value, NumberStyles.Integer, null, out num);

            bool arxiv = true;
            bool.TryParse(HttpContext.Request.Query
               .FirstOrDefault(q => string.Compare(q.Key, "arxiv", true) == 0)
               .Value, out arxiv);

            bool doaj = true;
            bool.TryParse(HttpContext.Request.Query
               .FirstOrDefault(q => string.Compare(q.Key, "doaj", true) == 0)
               .Value, out doaj);


            return new InspirationRequest(query, num, arxiv, doaj);
        }

        private async Task<dynamic> GetResponse(InspirationRequest data)
        {
            Console.WriteLine($"Request Fresh Data, query: {data.Queries.FirstOrDefault()}");
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(data), 
                Encoding.UTF8, "application/json");
            var response = await client.PostAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(responseContent);
        }

    }
}
