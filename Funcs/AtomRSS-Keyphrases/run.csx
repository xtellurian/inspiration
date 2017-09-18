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
        if(feed==null) continue;
        tasks.Add(ProcessFeed(feed, log));
    }

    Task.WaitAll(tasks.ToArray());    
    
    return req.CreateResponse(HttpStatusCode.OK, data as JObject);
}

private static async Task<dynamic> ProcessFeed(dynamic feed, TraceWriter log)
{
    // this data structure has changed
    var items = feed.feed.entry as JArray;
    if(items == null) return feed;
    log.Info("items was not null");
    foreach(var i in items){
        log.Info(i["id"].Value<string>());
    }
    var titles = items.ToDictionary(i=>i["id"].Value<string>(), i=>i["title"].Value<string>());
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