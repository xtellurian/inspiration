using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AngularInspiration.Model.Contract
{
    public interface IInspiration
    {
        string Id {get;set;}
        string Title {get;}
        string Summary {get;}
        string Link {get;}

    }
}

// DOAJ =  https://doaj.org/article/<article-id>
