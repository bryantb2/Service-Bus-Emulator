using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBus
{
    public class ObservableQueue<T> : EventQueue<T>, IObservable<BusEvent<T>>
    {
        // fields
        private List<IObserver<BusEvent<T>>> observers = new List<IObserver<BusEvent<T>>>();

        public ObservableQueue() : base() { }

        public IDisposable Subscribe(IObserver<BusEvent<T>> observer)
        {

            if (!this.observers.Contains(observer))
                this.observers.Add(observer);
            return new Unsubscriber(this.observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private IObserver<BusEvent<T>> _observer;
            private List<IObserver<BusEvent<T>>> _observers;

            public Unsubscriber(List<IObserver<BusEvent<T>>> observers, IObserver<BusEvent<T>> observer)
            {
                this._observers = observers;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
