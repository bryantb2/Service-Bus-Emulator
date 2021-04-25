using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace ServiceBus
{
    public class ObservableQueue<T> : EventQueue<T>, IObservable<BusEvent<T>>
    {
        // fields
        private List<IObserver<BusEvent<T>>> observers = new List<IObserver<BusEvent<T>>>();
        private ObservableCollection<BusEvent<T>> observableEvents;

        public ObservableQueue() : base() 
        {
            // wraps an observable around base class event list
            this.observableEvents = new ObservableCollection<BusEvent<T>>(this.Events);
            this.observableEvents.CollectionChanged += new NotifyCollectionChangedEventHandler(this.HandleCollectionChange);
        }

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
                this._observer = observer;
                this._observers = observers;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        private void HandleCollectionChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                this.NotifyChanges();
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                this.NotifyChanges();
        }

        private void NotifyChanges()
        {
            foreach(var observer in this.observers)
            {
                var busEvent = this.GetNewestUnprocessedEvent();
                observer.OnNext(busEvent);
            }
        }
    }
}
