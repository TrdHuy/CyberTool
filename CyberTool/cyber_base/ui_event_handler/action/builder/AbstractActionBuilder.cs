﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.builder
{
    public abstract class AbstractActionBuilder : IActionBuilder
    {
        protected BuilderLocker _locker = new BuilderLocker(BuilderStatus.Unlock, false);

        public virtual BuilderLocker Locker { get => _locker; set => _locker = value; }

        public abstract IAction? BuildAlternativeActionWhenFactoryIsLock(string keyTag, object? dataTransfer);

        public abstract IAction? BuildMainAction(string keyTag, object? dataTransfer);

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
