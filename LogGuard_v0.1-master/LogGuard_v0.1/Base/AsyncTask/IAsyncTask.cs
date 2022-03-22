using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.AsyncTask
{
    public interface IAsyncTask
    {
        /// <summary>
        /// Thời gian delay (theo millisecon) khi thực hiện async action
        /// </summary
        long DelayTime { get; }

        /// <summary>
        /// Kết quả trả về sau khi triển khai async action
        /// </summary
        AsyncTaskResult Result { get; }

        /// <summary>
        /// Xác định xem liệu task này có thể triển khai được hay không
        /// </summary>
        Func<bool> CanExecute { get; }

        /// <summary>
        /// Triển khai task cho 1 đối tượng  được định nghĩa trước
        /// </summary>
        Func<Task<AsyncTaskResult>> Execute { get; }

        /// <summary>
        /// Triển khai task cho 1 đối tượng  được định nghĩa trước
        /// </summary>
        Func<CancellationToken, Task<AsyncTaskResult>> CancelableExecute { get; }


        /// <summary>
        /// Xử lý call back sau khi async task được triển khai 
        /// </summary>
        Action<AsyncTaskResult> CallbackHandler { get; }

        /// <summary>
        /// Task chính đã được thực hiện xong chưa
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Callback đã được thực hiện xong chưa
        /// </summary>
        bool IsCompletedCallback { get; }

        /// <summary>
        /// Kiểm tra Task chính có bị hủy hay không
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Trigger a notify when IsCompleted changed
        /// </summary>
        event IsCompletedChangedHandler OnCompletedChanged;

        /// <summary>
        /// Trigger a notify when Iscanceld changed
        /// </summary>
        event IscanceldChangedHandler OncanceldChanged;
    }

    public delegate void IsCompletedChangedHandler(object sender, bool oldValue, bool newValue);
    public delegate void IscanceldChangedHandler(object sender, bool oldValue, bool newValue);
}
