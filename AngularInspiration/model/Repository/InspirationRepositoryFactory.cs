using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using AngularInspiration.Model.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AngularInspiration.Model
{
    
    public class InspirationRespositoryFactory : IRepoFactory
    {
        private List<IInspirationRepository> _repos; // just keeping track of them for now
        public IList<IInspirationRepository> MakeAllRepositories()
        {
             _repos = new List<IInspirationRepository>();
            _repos.Add(new arXivRepo());
            _repos.Add(new DoajRepo());
            return _repos;
        }


    }
}

