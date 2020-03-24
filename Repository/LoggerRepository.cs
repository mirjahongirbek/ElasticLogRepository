using Nest;
using RepositoryCore.Interfaces;
using System;
using System.Runtime.CompilerServices;
using ElasticLogRepository.Models;
using RepositoryCore.Enums.Enum;
using System.Linq;

namespace ElasticLogRepository.Repository
{
    public class LoggerRepository<T, TKey> : ILoggerCoreRepository<T, TKey>
        where T : class, IEntity<TKey>
    {

        ElasticClient _client;
        ElasticClient _stet;
        ElasticClient _log;
        protected bool _event;
        protected string _name;
        public LoggerRepository()
        {
            //LogConfig.ConnectionString
            var log = LogConfig.DefaultIndex + typeof(T).Name;
            _name = typeof(T).Name;
            var url = new Uri(LogConfig.ConnectionString);
            var settings = new ConnectionSettings(url).DefaultIndex(log);
            var statisSettings = new ConnectionSettings(url).DefaultIndex(LogConfig.DefaultStatistic);
            _stet = new ElasticClient(statisSettings);
            _event = LogConfig.AddEvent;

        }
        public void AddDocument(T document)
        {
            Change(document, CrudStatus.Create);
        }
        public EventAction GetEvent
        {
            get
            {
                 var gEvent = _stet.Search<EventAction>(m => m.Query(q => q.Match(i => i.Field(d => d.TypeName == _name)))).Documents.FirstOrDefault();
                if (gEvent == null)
                {
                    gEvent = new EventAction()
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeName = _name
                    };
                    _stet.IndexDocument(gEvent);
                }
                return gEvent;
            }
        }
        private void SaveChanges(EventAction action)
        {
            _stet.Update<EventAction>(DocumentPath<EventAction>.Id(action.Id), m => m.Doc(action));
        }
        public void Delete(T document)
        {
            Change(document, CrudStatus.Delete);

        }
        private void UpdateElastic(EventModal<T, TKey> document)
        {
            _client.Update(DocumentPath<EventModal<T, TKey>>.Id(document.Id), m => m.Doc(document));            
        }
        private void Change(T document, CrudStatus status)
        {

            var gets = GetEvent;
            switch (status)
            {
                case CrudStatus.Create:
                    {
                        gets.Count += 1;
                        EventModal<T, TKey> modal = new EventModal<T, TKey>();
                        modal.Model = document;
                        modal.Id = document.Id.ToString();
                        _client.IndexDocument(modal);
                        gets.Count++;
                    }
                    break;
                case CrudStatus.Update:
                    {
                        var modal = GetById(document.Id.ToString());
                        if (modal != null)
                        {
                            modal.List.Add(modal.Model);
                            modal.Model = document;
                            UpdateElastic(modal);
                        }
                        gets.Changes++;
                    }
                    break;
                case CrudStatus.Delete:
                    {
                        var doc = GetById(document.Id.ToString());
                        if (doc != null)
                        {
                            doc.CrudStatus = CrudStatus.Delete;
                        }
                        UpdateElastic(doc);
                        gets.Deleted++;
                    }
                    break;
            }
            SaveChanges(gets);
            //if (_event)
            //{
            //    EventModal<T, TKey> modal = new EventModal<T, TKey>();
            //    modal.Model = document;
            //    modal.Id = document.Id.ToString();
            //    _client.IndexDocument(modal);
            //}

        }
        public EventModal<T, TKey> GetById(string id)
        {
            var doc = _client.Search<EventModal<T, TKey>>(m => m.Query(q => q.Match(i => i.Field(n => n.Id == id).Field(n => n.Name == _name))));
            return doc.Documents.FirstOrDefault();
        }

        public void Update(T document)
        {
            Change(document, CrudStatus.Update);
        }


        public void CatchError(string text, long time, object obj, Exception exception, string methodName, string GUID, [CallerLineNumber] int linenumber = 0)
        {
            ActionModal modal = new ActionModal()
            {
                Id = string.IsNullOrEmpty(GUID) ? Guid.NewGuid().ToString() : GUID,
                DateTime = DateTime.Now,
                Method = methodName,
                 Obj=obj, Text= text, Time= time,
                  Name= _name
            };
            _log.IndexDocument(modal);
        }


        public void Logging(string text, long time, object obj, string methodName, [CallerLineNumber] int linenumber = 0)
        {
            ActionModal modal = new ActionModal()
            {
                Id = Guid.NewGuid().ToString(),
                DateTime = DateTime.Now,
                Method = methodName,
                Obj = obj,
                Text = text,
                Time = time,
                Name = _name
            };
            _log.IndexDocument(modal);
        }

        public void StartFunction(string text, long time, object obj, string methodName, [CallerLineNumber] int linenumber = 0)
        {
            ActionModal modal = new ActionModal()
            {
                 Text= text, Time= time, Obj=obj, Method= methodName, DateTime= DateTime.Now, Id= Guid.NewGuid().ToString()
            };
            _log.IndexDocument(modal);

        }

    }
}
