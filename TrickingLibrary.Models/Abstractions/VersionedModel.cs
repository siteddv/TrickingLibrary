using System;

namespace TrickingLibrary.Models.Abstractions
{
    public class VersionedModel : BaseModel<int>
    {
        public int Version { get; set; }
        public bool Active { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}