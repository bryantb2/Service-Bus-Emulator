using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBus.worker
{
    public class ObserverWorker<T> : Worker<T>, IObserver<BusEvent<T>>
    {
        public ObserverWorker(
            Action<BusEvent<T>> processingAction, 
            Action<BusEvent<T>> cleanupAction
        ) : base(processingAction, cleanupAction) { }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Error in worker observable: " + error.ToString());
        }

        public void OnNext(BusEvent<T> value)
        {
            this.ProcessEvent(value);
        }
    }
}
