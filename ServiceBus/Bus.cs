using System;
using System.Collections.Generic;

namespace ServiceBus
{
    public class Bus<T>
    {
        // fields
        private static int eventIdCounter = 0; 
        private string name = "";
        private List<EventQueue<T>> eventQueues = new List<EventQueue<T>>();

        public Bus(string name)
        {
            this.name = name;
            this.BusId = GetBusIdAndIncrement();
        }

        // static methods
        public static int GetBusIdAndIncrement()
        {
            var currentId = eventIdCounter;
            eventIdCounter++;
            return currentId;
        }

        // properties
        public int BusId { get; set; }

        public string Name { 
            get  {
                return this.name;
            }
        }

        public List<EventQueue<T>> Queues
        {
            get
            {
                return this.eventQueues;
            }
        }

        // methods
        public EventQueue<T> GetQueueById(int id)
        {
            return this.eventQueues.Find(q => q.QueueId == id);
        }

        public void AddQueue(EventQueue<T> queue)
        {
            this.eventQueues.Add(queue);
        }

        public void RegisterQueue(EventQueue<T> queue)
        {
            this.eventQueues.Add(queue);
        }

        public bool RemoveQueue(EventQueue<T> queue)
        {
            var didRemove = this.eventQueues.Remove(queue);
            return didRemove;
        }

        public EventQueue<T> RemoveQueueById(int id)
        {
            var qToRemove = this.eventQueues.Find(q => q.QueueId == id);
            if (qToRemove != null)
            {
                this.eventQueues.Remove(qToRemove);
            }
            return qToRemove;
        }
    }
}
