using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class Worker<T>
    {
        // fields
        private static int eventIdCounter = 0;
        private Action<BusEvent<T>> processingAction;
        private Action<BusEvent<T>> cleanupAction;

        // properties
        public int WorkerId { get; }

        protected Worker(Action<BusEvent<T>> processingAction, Action<BusEvent<T>> cleanupAction)
        {
            this.processingAction = processingAction;
            this.cleanupAction = cleanupAction;
            this.WorkerId = GetWorkerIdAndIncrement();
        }

        protected Task<bool> ProcessEvent(BusEvent<T> evnt)
        {
            try
            {
                // run event procces
                this.processingAction(evnt);
                this.cleanupAction(evnt);
                return Task.FromResult(true);
            } catch(Exception err)
            {
                Console.WriteLine("Worker " + this.WorkerId.ToString() + " has thrown an exception: " + err.ToString());
                return Task.FromResult(false);
            }
        }

        public static int GetWorkerIdAndIncrement()
        {
            var currentId = eventIdCounter;
            eventIdCounter++;
            return currentId;
        }
    }
}
