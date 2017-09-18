#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

private static string serviceEndpoint = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/";

public static async Task<Dictionary<string, List<string>>> GetKeyphrases(IDictionary<string, string> texts, TraceWriter log)
{
    var apiKey = ConfigurationManager.AppSettings["cognitive_api_key"];
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
    log.Info($"There are {texts.Keys.Count()} texts");
    var request = new TextRequest();
    foreach(var kvp in texts){
        if(!string.IsNullOrEmpty(kvp.Value)){
            request.Documents.Add(new TextDocument(kvp.Key, kvp.Value));
        }
    }

    log.Info($"Created Requests with {request.Documents.Count()} documents");
    var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
    var response = await httpClient.PostAsync($"{serviceEndpoint}keyPhrases", content).ConfigureAwait(false);

    var body = JObject.Parse(await response.Content.ReadAsStringAsync());
    var ff = body["documents"].Children().ToList();
    var result = new Dictionary<string, List<string>>();
    foreach(var doc in ff)
    {
        var id = doc["id"].Value<string>();
        result[id] = new List<string>();
       // log.Info($"Id is {id}");
        foreach(var s in doc["keyPhrases"])
        {
            //log.Info($"{id} - {s}");
            result[id].Add(s.Value<string>());
        }
    }
    log.Info("Returning from GetKeyphrases");
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