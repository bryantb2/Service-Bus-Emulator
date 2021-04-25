using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBus
{
    public class EventQueue<T>
    {
        // fields
        private static int eventIdCounter = 0;
        private List<BusEvent<T>> events = new List<BusEvent<T>>();

        public EventQueue() {
            this.QueueId = GetEventIdAndIncrement();
        }

        // properties
        public int QueueId { get; set; }

        public List<BusEvent<T>> Events
        {
            get
            {
                return this.events;
            }
        }

        // operator overrides
        public BusEvent<T> this[int key]
        {
            get
            {
                if (key >= 0 && 0 < this.events.Count)
                    return this.events[key];
                return null;
            }
        }

        // static methods
        public static int GetEventIdAndIncrement()
        {
            var currentId = eventIdCounter;
            eventIdCounter++;
            return currentId;
        }

        // methods
        public BusEvent<T> GetNewestUnprocessedEvent()
        {
            // scan queue for most recent event
            for(var i = this.events.Count - 1; i >= 0; i--)
            {
                var currentEvent = this.events[i];
                if (currentEvent.Processing && !currentEvent.Processed)
                    return currentEvent;
            }
            return null;
        }

        public BusEvent<T> GetEventById(int id)
        {
            return this.events.Find(evnt => evnt.EventId == id);
        }

        public void PublishEvent(T eventPayload)
        {
            var busEvent = new BusEvent<T>(){
                EventId = GetEventIdAndIncrement(),
                Processing = false,
                Processed = false,
                CreationTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Payload = eventPayload
            };
            // add to queue
            this.events.Add(busEvent);
        }

        public BusEvent<T> ClearEvent(int id)
        {
            var index = this.events.FindIndex(e => e.EventId == id);
            if (index >= 0)
            {
                var e = this.events[index];
                events.Remove(e);
                return e;
            }
            return null;
        }

        public void ClearEvent(BusEvent<T> e)
        {
            events.Remove(e);
        }

        public bool GetStatusById(int eventId)
        {
            return this.events.Find((e) => e.EventId == eventId)?.Processing ?? false;
        }
    }
}
