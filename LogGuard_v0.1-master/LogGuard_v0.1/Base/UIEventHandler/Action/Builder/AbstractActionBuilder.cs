using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.UIEventHandler.Action.Builder
{
    public abstract class AbstractActionBuilder : IActionBuilder
    {
        protected BuilderLocker _locker = new BuilderLocker(BuilderStatus.Unlock, false);

        public virtual BuilderLocker Locker { get => _locker; set => _locker = value; }

        public abstract IAction BuildAlternativeActionWhenFactoryIsLock(string keyTag);

        public abstract IAction BuildMainAction(string keyTag);

        public void LockBuilder(BuilderStatus status)
        {
            Locker.IsLock = true;
            Locker.Status = status;
        }

        public void UnlockBuilder(BuilderStatus status)
        {
            Locker.IsLock = false;
            Locker.Status = status;
        }
    }
}
