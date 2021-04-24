using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBus
{
    public class BusEvent<T>
    {
        public int EventId { get; set; }
        public bool Processing { get; set; }
        public bool Processed { get; set; }
        public long CreationTime { get; set; }
        public T Payload { get; set; }
    }
}
