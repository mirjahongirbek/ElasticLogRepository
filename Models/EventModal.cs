using Nest;
using RepositoryCore.Enums.Enum;
using RepositoryCore.Interfaces;
using System;
using System.Collections.Generic;

namespace ElasticLogRepository.Models
{
    public class EventModal<T, TKey>
         where T : class, IEntity<TKey>
    {
        [Text(Name = "name", Index = true)]
        public string Name { get; set; }
        [Text(Name = "id", Index = true)]
        public string Id { get; set; }
        public T Model { get; set; }
        public List<T> List { get; set; } = new List<T>();
        public CrudStatus CrudStatus { get; set; }
    }
    public class ActionModal : IEntity<string>
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public long Time { get; set; }
        public object Obj { get; set; }
        public string Method { get; set; }
        
    }


}
