using LogGuard_v0._1.Base.Observable;
using LogGuard_v0._1.Windows.BaseWindow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.BaseWindow.Utils
{
    public abstract class BasePageController : BaseObservable<PageVO>
    {
        protected Lazy<PageVO> NonePage = new Lazy<PageVO>(() =>
           new PageVO(new Uri("", UriKind.Relative), 0));

        public PageVO CurrentPageOV { get; set; }
        public PageSource CurrentPageSource { get; protected set; } = PageSource.NONE;
        public PageSource PreviousePageSource { get; protected set; } = PageSource.NONE;

        public abstract void UpdateCurrentPageSource(PageSource pageSource);
        public abstract void UpdatePageOVUri(Uri uri);

    }
}
