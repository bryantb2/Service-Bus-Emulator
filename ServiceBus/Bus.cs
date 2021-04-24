using System;
using System.Collections.Generic;

namespace ServiceBus
{
    public class Bus<T>
    {
        private string name;
        private List<BusEvent<T>> events;
        private List<Worker<T>> workers;
        private Dictionary<Worker<T>, List<IObservable<BusEvent<T>>>> queue;

        // properties
        public string Name { 
            get  {
                return this.name;
            }
        }
        public List<BusEvent<T>> Events { 
            get {
                return this.events;
            }
        }
        public List<Worker<T>> Workers { 
            get {
                return this.workers;
            } 
        }

        public Bus(string name)
        {
            this.name = name;
        }
        
        // methods
        public void PublishEvent(BusEvent<T> e)
        {
            this.events.Add(e);
        }

        public void RegisterWorker(Worker<T> worker)
        {
            //var subscriber = 
            //this.workers.Add(worker);
            throw new NotImplementedException();
        }
    }
}
