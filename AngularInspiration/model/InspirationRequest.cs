using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AngularInspiration.Model
{
    
    public class InspirationRequest
    {
        public InspirationRequest()
        {

        }
        public InspirationRequest (string query, int num, bool arxv, bool doaj)
        {
            _queryString = query;
            Queries = new List<string>{query};
            Num = num;
            ArXiv = arxv;
            DOAJ = doaj;
        }
        private string _queryString;

        [JsonProperty("queries")]
        public IEnumerable<string> Queries {get;set;}
        [JsonProperty("num")]
        public int Num {get;set;}

        [JsonProperty("arXiv")]
        public bool ArXiv {get;set;}
        [JsonProperty("DOAJ")]
        public bool DOAJ {get;set;} 


        public bool IsValid ()
        {
            return !string.IsNullOrEmpty(_queryString)
                && !string.IsNullOrWhiteSpace(_queryString)
                && Num > 0;
        }
        public override bool Equals(object obj) 
        {
            var req = obj as InspirationRequest;
            if(req == null) return false;

            return string.Equals(req._queryString, this._queryString)
                && Num == req.Num
                && ArXiv == req.ArXiv
                && DOAJ == req.DOAJ;
        }

        public override string ToString()
        {
            return _queryString;
        }
    }
}
