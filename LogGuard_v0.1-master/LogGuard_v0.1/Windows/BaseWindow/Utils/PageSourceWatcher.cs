using LogGuard_v0._1.Base.Observable;
using LogGuard_v0._1.Windows.BaseWindow.Models;

namespace LogGuard_v0._1.Windows.BaseWindow.Utils
{
    public class PageSourceWatcher : IObserver<PageVO>
    {
        private System.Action<PageVO> OnPageSourceChange;

        internal PageSourceWatcher(System.Action<PageVO> onSourceChange)
        {
            OnPageSourceChange = onSourceChange;
        }
        public void Update(PageVO value)
        {
            OnPageSourceChange?.Invoke(value);
        }
    }
}
