#r "Newtonsoft.Json"
using System.Net;
using Newtonsoft.Json.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("Function called");

    int num = 10; // default

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    var queries = data?.queries as JArray;
    if(data?.num != null) num = data?.num;
    log.Info(data?.queries.GetType().ToString());
    log.Info($"Got values from req body: {data?.queries} {data?.num}");

    var feeds = new List<dynamic>();
    var tasks = new List<Task<dynamic>>();

    foreach (var val in queries)
    {
        var query = val.Value<string>();
        log.Info($"Searching for {query} with num: {num}"); 
        tasks.Add(GetFeed(query, num, log));
    }
    Task.WaitAll(tasks.ToArray());
    foreach(var t in tasks)
    {
        feeds.Add(t.Result);
    }
    

    return queries == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a query as  string")
        : req.CreateResponse(HttpStatusCode.OK, new {feeds = feeds});
}


private static string uri = "https://doaj.org/api/v1/search/articles/{query}?page={start}&pageSize={maxResults}";

private static async Task<dynamic> GetFeed(string query, int num, TraceWriter log)
{
    var client = new HttpClient();
    var finalUri = uri.Replace("{query}", query)
        .Replace("{maxResults}",num.ToString())
        .Replace("{start}","0"); // temp solution here
    
    var response = await client.GetAsync(finalUri);
    dynamic data = await response.Content.ReadAsAsync<object>();
    return data;
}