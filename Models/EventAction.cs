
using Nest;

namespace ElasticLogRepository.Models
{
    public class EventAction
    {
        [Text(Name = "id", Index = true)]
        public string Id { get; set; }
        [Text(Name = "type_name", Index = true)]
        public string TypeName { get; set; }
        public long Count { get; set; }
        public long Changes { get; set; }
        public long Deleted { get; set; }

    }


}
