using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.Observable
{
    public class BaseObservable : IObservable
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyChange()
        {
            foreach (IObserver observer in _observers)
            {
                observer.Update();
            }
        }
    }
}
