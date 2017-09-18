#r "Newtonsoft.Json"
using System.Net;
using System.Xml;
using Newtonsoft.Json;
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
    log.Info($"Waiting for {tasks.Count} tasks to complete");
    Task.WaitAll(tasks.ToArray());
    log.Info("Tasks Completed");
    foreach(var t in tasks)
    {
        feeds.Add(t.Result);
    }
    
    return req.CreateResponse(HttpStatusCode.OK, new {feeds = feeds});
}

private static string uri = "http://export.arxiv.org/api/query?search_query=all:{query}" + "&start={start}&max_results={maxResults}";

private static async Task<dynamic> GetFeed(string query, int num, TraceWriter log)
{
    var client = new HttpClient();
    var finalUri = uri.Replace("{query}", query)
        .Replace("{maxResults}",num.ToString())
        .Replace("{start}","0"); // temp solution here

    var response = await client.GetAsync(finalUri);
    var xml = await response.Content.ReadAsStringAsync();
    XmlDocument doc = new XmlDocument();
    doc.LoadXml(xml);
    string jsonText = JsonConvert.SerializeXmlNode(doc);
    return JsonConvert.DeserializeObject(jsonText);

  //  dynamic data = await response.Content.ReadAsAsync<object>();
 //   return data;
}
