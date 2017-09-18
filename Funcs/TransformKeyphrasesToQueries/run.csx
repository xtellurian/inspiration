#r "Newtonsoft.Json"
using System.Net;
using Newtonsoft.Json.Linq;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    string numString = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "num", true) == 0)
        .Value;
    
    var num = 5; // default;
    if(!string.IsNullOrEmpty(numString))
    {
        num = int.Parse(numString);
    }

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    var queryStrings = new List<string>();
    if(data?.feeds != null)
    {
        log.Info("data.feeds is not null");
        foreach(var feed in data?.feeds)
        {
            if(feed == null || feed.KeyPhrases == null) continue;
            log.Info("inner feed is not null AND KeyPhrases is not null");
            foreach(var dic in feed.KeyPhrases)
            {
                foreach(var array in dic.Children())
                {
                    foreach(var v in array)
                    {
                        queryStrings.Add(v.ToString());
                    }
                }
            }
        }
    } 
    

    return req.CreateResponse(HttpStatusCode.OK, new { queries = queryStrings, num = num });
}