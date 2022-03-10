using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.Observable
{
    public interface IObservable
    {
        void Attach(IObserver observer);

        void Detach(IObserver observer);

        void NotifyChange();
    }
}
