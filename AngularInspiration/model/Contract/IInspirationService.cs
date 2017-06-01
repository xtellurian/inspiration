using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AngularInspiration.Model.Contract
{
    public interface IInspirationService
    {
        InspirationSession NewSession ();
    }
}

// DOAJ =  https://doaj.org/article/<article-id>
