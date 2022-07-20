using cyber_base.implement.attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.ui_event_handler.action.builder
{
    public interface IActionBuilder
    {
        IAction? BuildAlternativeActionWhenFactoryIsLock(string keyTag);

        IAction? BuildMainAction(string keyTag);

        void LockBuilder(BuilderStatus status = BuilderStatus.Default);
        void UnlockBuilder(BuilderStatus status = BuilderStatus.Default);

        BuilderLocker Locker { get; set; }
    }

    public class BuilderLocker
    {
        public BuilderStatus Status;
        public bool IsLock;

        public BuilderLocker(BuilderStatus status, bool key)
        {
            Status = status;
            IsLock = key;
        }
    }

    public enum BuilderStatus
    {
        [StringValue("...")]
        Default = 0,

        [StringValue("Tác vụ đang được xử lý, vui lòng đợi trong vài giây!")]
        TaskHandling = 1,

        [StringValue("Tác vụ hiện không khả dụng, vui lòng chọn tác vụ khác!")]
        NotAvailable = 2,

        [StringValue("Mở khóa tác vụ!")]
        Unlock = 3,

        [StringValue("Tác vụ đang được xử lý, vui lòng đợi trong giây lát!")]
        TaskHandlingButCanDispose = 4,
    }
}
