#load "textAnalytics.csx"
using System.Net;
using Newtonsoft.Json.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();
    var tasks = new List<Task>();
    foreach(var feed in data?.feeds)
    {
        tasks.Add(ProcessFeed(feed, log));
    }

    Task.WaitAll(tasks.ToArray());    
    
    return req.CreateResponse(HttpStatusCode.OK, data as JObject);
}

private static async Task<dynamic> ProcessFeed(dynamic feed, TraceWriter log)
{
    var results = feed.results as JArray;
    foreach(var i in results){
        log.Info(i["bibjson"]["title"].Value<string>());
    }
    var titles = results.ToDictionary(i=>i["id"].Value<string>(), i=>i["bibjson"]["title"].Value<string>());
    if(titles.Count() == 0 )
    {
        feed.KeyPhrases = null;
        return feed;
    }
    var result = await GetKeyphrases(titles, log);

    log.Info($"{result.Keys.Count()}");

    feed.KeyPhrases = JToken.FromObject(result);
    return feed;
}