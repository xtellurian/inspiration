using System;
using System.Collections.Generic;
using AngularInspiration.Model.Contract;
using Newtonsoft.Json;

namespace AngularInspiration.Model
{
    public class InspirationService : IInspirationService
    {
        IRepoFactory _repoFactory;
        ITextAnalyticsService _textAnalytics;
        public InspirationService(IRepoFactory repoFactory, ITextAnalyticsService textAnalytics)
        {
            _repoFactory = repoFactory;
            _textAnalytics = textAnalytics;
        }

        public InspirationSession NewSession ()
        {
            return new InspirationSession(_repoFactory.MakeAllRepositories(), _textAnalytics);
        }
    }
}


