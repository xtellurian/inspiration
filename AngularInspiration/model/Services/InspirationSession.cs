using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularInspiration.Model.Contract;
using Newtonsoft.Json;

namespace AngularInspiration.Model
{
    public class InspirationSession 
    {
        private ConcurrentDictionary<string, ConcurrentQueue<IInspiration>> _keyphraseLookup 
            = new ConcurrentDictionary<string, ConcurrentQueue<IInspiration>>();
        private Random _ran;
        private List<IInspirationRepository> _repos;
        private ITextAnalyticsService _textAnalytics;
        public InspirationSession(IEnumerable<IInspirationRepository> repos, ITextAnalyticsService textAnalytics)
        {
            _repos = new List<IInspirationRepository>(repos);
            _textAnalytics = textAnalytics;
            _ran = new Random((int) DateTime.Now.Ticks); 
        }

        private List<Task> _backgroundtasks = new List<Task>();
        public async Task<IInspiration> NextRandom(string text, int bredthFactor)
        {
            // do some caching too

            // first check keyphrase lookup
             //pick random repo
             // search repo
             if(_keyphraseLookup.ContainsKey(text) && _keyphraseLookup[text].Count > 0) // cache exists, lets use it
            {
                IInspiration result;
                if( _keyphraseLookup[text].TryDequeue(out result)){
                    return result;
                }
                else{
                    Console.WriteLine($"Failed to Dequeue, text: {text}");
                }
            }

            var keyPhrases = await GetRelatedKeyphrases(PickRandom(_repos),text, bredthFactor );

            // also run a task with all the keyPhrases to fill up our queue in the background
             foreach(var t in _backgroundtasks) // remove complete
            {
                if(t.IsCompleted) _backgroundtasks.Remove(t);
            }
            
            _backgroundtasks.Add(FillQueue(text, _repos, keyPhrases, bredthFactor));


            var keyPhrase = PickRandom(keyPhrases);
            
            // research random repo with random keyphrase
            var repo = PickRandom(_repos);
            
            var results = await repo.Search(keyPhrase, bredthFactor);
            // cache results
            _keyphraseLookup.TryAdd(text, new ConcurrentQueue<IInspiration>());
            foreach(var r in results)
            {
                _keyphraseLookup[text].Enqueue(r);
            }
            

            return PickRandom(results);
            
        }

        private async Task FillQueue(string text, List<IInspirationRepository> repos, IEnumerable<string> keyPhrases, int num)
        {
            var tasks = new List<Task>();
            foreach(var phrase in keyPhrases)
            {
                foreach(var repo in repos)
                {
                    var result = await repo.Search(phrase, num);
                    foreach(var i in result)
                    {
                        if(_keyphraseLookup.TryAdd(text, new ConcurrentQueue<IInspiration>())){
                            Console.WriteLine($"Added new key to dictionary: {text}");
                        }
                        _keyphraseLookup[text].Enqueue(i);
                    }
                }
            }
        }

        private async Task<List<string>> GetRelatedKeyphrases(IInspirationRepository repo, string text, int num = 5, int page = 0)
        {
            var results = await repo.Search(text,num,page );
            var keyPhrases = await _textAnalytics.KeyPhrases(results, t => t.Id, t=> t.Title + " " + t.Summary);
            var collection = new List<string>();
            foreach(var l in keyPhrases.Values)
            {
                collection.AddRange(l);
            }

            return collection;
        }

        private T PickRandom<T>(List<T> collection) 
        {
            var len = collection.Count;
            return collection[_ran.Next(len)];
        }

        private IInspiration PickRandom(InspirationCollection collection)
        {
            var len = collection.Count;
            return collection[_ran.Next(len )];
        }

    }
}


