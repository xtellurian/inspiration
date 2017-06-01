using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using AngularInspiration.Model.Contract;

namespace AngularInspiration.Model
{
    public class InspirationCollection : IEnumerable<IInspiration>
    {
        public InspirationCollection()
        {
            _backingCollection = new List<IInspiration>();
        }
        public int TotalMatching {get;set;}
        public string SearchTerm {get;set;}
        public int Count =>  _backingCollection.Count;
        public void Add (IInspiration inspiration)
        {
            if(_backingCollection== null) _backingCollection = new List<IInspiration>();
            _backingCollection.Add(inspiration);
        }
        private List<IInspiration> _backingCollection;

        public IEnumerator<IInspiration> GetEnumerator()
        {
            return _backingCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _backingCollection.GetEnumerator();
        }

        public IInspiration this[int i]
        {
            get { return _backingCollection[i]; }
            set { _backingCollection[i] = value; }
        }
    }
}
