using LogGuard_v0._1.Utils.Definitions;
using LogGuard_v0._1.Windows.BaseWindow.Models;
using LogGuard_v0._1.Windows.BaseWindow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.MainWindow.Utils
{
    public class MSW_PageController : BasePageController
    {
        private static MSW_PageController _instance;

        private Lazy<PageVO> LogGuardPage = new Lazy<PageVO>(() =>
           new PageVO(
               new Uri(LogGuardDefinition.LOG_GUARD_PAGE_URI_ORIGINAL_STRING, UriKind.Relative),
               LogGuardDefinition.LOG_GUARD_PAGE_LOADING_DELAY_TIME));

        private Lazy<PageVO> UnderConstructionPage = new Lazy<PageVO>(() =>
          new PageVO(
              new Uri(LogGuardDefinition.UNDER_CONSTRUCTION_PAGE_URI_ORIGINAL_STRING, UriKind.Relative),
              LogGuardDefinition.UNDER_CONSTRUCTION_PAGE_LOADING_DELAY_TIME));

        private MSW_PageController()
        {
            CurrentPageOV = new PageVO(new Uri(LogGuardDefinition.UNDER_CONSTRUCTION_PAGE_URI_ORIGINAL_STRING, UriKind.Relative),
              LogGuardDefinition.UNDER_CONSTRUCTION_PAGE_LOADING_DELAY_TIME);
        }

        public override void UpdateCurrentPageSource(PageSource pageNum)
        {
            PreviousePageSource = CurrentPageSource;
            CurrentPageSource = pageNum;

            switch (pageNum)
            {
                
                case PageSource.LOG_GUARD_PAGE:
                    CurrentPageOV.PageUri = LogGuardPage.Value.PageUri;
                    CurrentPageOV.LoadingDelayTime = LogGuardPage.Value.LoadingDelayTime;
                    break;
                case PageSource.UNDER_CONSTRUCTION_PAGE:
                    CurrentPageOV.PageUri = UnderConstructionPage.Value.PageUri;
                    CurrentPageOV.LoadingDelayTime = UnderConstructionPage.Value.LoadingDelayTime;
                    break;
                default:
                    CurrentPageOV.PageUri = UnderConstructionPage.Value.PageUri;
                    CurrentPageOV.LoadingDelayTime = UnderConstructionPage.Value.LoadingDelayTime;
                    break;
            }
            NotifyChange(CurrentPageOV);
        }

        public override void UpdatePageOVUri(Uri uri)
        {
            var x = "/" + uri.OriginalString;
            PreviousePageSource = CurrentPageSource;
            switch (x)
            {
                case LogGuardDefinition.LOG_GUARD_PAGE_URI_ORIGINAL_STRING:
                    CurrentPageSource = PageSource.LOG_GUARD_PAGE;
                    CurrentPageOV.PageUri = LogGuardPage.Value.PageUri;
                    CurrentPageOV.LoadingDelayTime = LogGuardPage.Value.LoadingDelayTime;
                    break;
                case LogGuardDefinition.UNDER_CONSTRUCTION_PAGE_URI_ORIGINAL_STRING:
                    CurrentPageSource = PageSource.UNDER_CONSTRUCTION_PAGE;
                    CurrentPageOV.PageUri = UnderConstructionPage.Value.PageUri;
                    CurrentPageOV.LoadingDelayTime = UnderConstructionPage.Value.LoadingDelayTime;
                    break;
                default:
                    CurrentPageSource = PageSource.UNDER_CONSTRUCTION_PAGE;
                    CurrentPageOV.PageUri = UnderConstructionPage.Value.PageUri;
                    CurrentPageOV.LoadingDelayTime = UnderConstructionPage.Value.LoadingDelayTime;
                    break;
            }

            NotifyChange(CurrentPageOV);
        }


        public static MSW_PageController Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MSW_PageController();
                }
                return _instance;
            }
        }
    }
}
