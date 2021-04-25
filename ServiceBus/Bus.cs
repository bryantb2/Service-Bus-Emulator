using System;
using System.Collections.Generic;

namespace ServiceBus
{
    public class Bus<T>
    {
        // fields
        private static int eventIdCounter = 0;
        private string name = "";
        private List<ObservableQueue<T>> eventQueues = new List<ObservableQueue<T>>();
        private Dictionary<int, IDisposable> workerSubscriptions = new Dictionary<int, IDisposable>();

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
                return this.eventQueues as List<EventQueue<T>>;
            }
        }

        // methods
        public EventQueue<T> GetQueueById(int id)
        {
            return this.FindQueueById(id);
        }

        public EventQueue<T> RegisterNewQueue()
        {
            // create and register queue
            var newQueue = new ObservableQueue<T>();
            this.eventQueues.Add(newQueue);
            // cast to base class
            return newQueue;
        }

        /**
         * Returns worker id
         */
        public int AttachWorkerToQueue(
            int queueId, 
            Action<BusEvent<T>> processingAction,
            Action<BusEvent<T>> cleanupAction
        )
        {
            var qToAttach = this.FindQueueById(queueId);
            if (qToAttach != null)
            {
                // create and attach to queue
                var newWorker = new ObserverWorker<T>(processingAction, cleanupAction);
                var unsubscribe = qToAttach.Subscribe(newWorker);
                // add unsubscribe to dictionary
                this.workerSubscriptions.Add(newWorker.WorkerId, unsubscribe);
                return newWorker.WorkerId;
            }
            return -1;
        }

        public void TerminateWorker(int workerId)
        {
            // find unsubscribe
            IDisposable unsubscribe;
            var exists = this.workerSubscriptions.TryGetValue(workerId, out unsubscribe);
            if (exists)
                unsubscribe.Dispose();

        }

        public EventQueue<T> TerminateQueue(int id)
        {
            //var didRemove = this.eventQueues.Remove(queue);
            var qToRemove = this.FindQueueById(id);
            if (qToRemove != null)
            {
                this.eventQueues.Remove(qToRemove);
            }
            return qToRemove;
        }

        private ObservableQueue<T> FindQueueById(int id)
        {
            var qToRemove = this.eventQueues.Find(q => q.QueueId == id);
            return qToRemove;
        }
    }
}
