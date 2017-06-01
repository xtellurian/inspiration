using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AngularInspiration.Model.Contract
{
    public interface IInspirationRepository
    {
        Task<InspirationCollection> Search(string matchPhrase, int num, int page = 0);
    }
}

