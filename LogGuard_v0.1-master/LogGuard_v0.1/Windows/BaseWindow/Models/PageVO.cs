using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Windows.BaseWindow.Models
{
    public class PageVO
    {
        public Uri PageUri { get; set; }

        public long LoadingDelayTime { get; set; }

        public PageVO(Uri uri, long delayTime = 2000)
        {
            PageUri = uri;
            LoadingDelayTime = delayTime;
        }
    }
}
