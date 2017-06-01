using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AngularInspiration.Model.Contract
{
    public interface ITextAnalyticsService
    {
        Task<Dictionary<string, List<string>>> KeyPhrases<T>(IEnumerable<T> inputTexts, Func<T, string> getId, Func<T, string> getText);
    }
}


