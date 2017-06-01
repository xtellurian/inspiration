

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AngularInspiration.Model.Contract
{
    public interface IRepoFactory
    {
        IList<IInspirationRepository> MakeAllRepositories();

    }
}

// DOAJ =  https://doaj.org/article/<article-id>
