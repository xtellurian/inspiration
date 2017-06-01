using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using AngularInspiration.Model.Contract;

namespace AngularInspiration.Model
{
    
    public class InspirationCard : IInspiration
    {

        public string Id {get;set;}
        public string Title {get;set;}
        public string Summary {get;set;}
        public string Link {get;set;}

    }
}

// DOAJ =  https://doaj.org/article/<article-id>
