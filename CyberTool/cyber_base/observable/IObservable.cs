﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.observable
{
    public interface IObservable<T>
    {
        void Subcribe(IObserver<T> observer);

        void Unsubcribe(IObserver<T> observer);

        void NotifyChange(T result);
    }
}
