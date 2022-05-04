using LogGuard_v0._1.AppResources.AttachedProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow.SourceFilter
{
    public interface IMechanicalSourceFilter : ISourceFilter
    {

        /// <summary>
        /// Nội dung điều kiện lọc
        /// </summary>
        /// <returns></returns>
        string FilterContent { get; set; }

        /// <summary>
        /// Kiểu engine cho bộ lọc hiện tại
        /// </summary>
        /// <returns></returns>
        FilterType CurrentFilterMode { get; set; }

        /// <summary>
        /// kiểm tra bộ lọc hiện tại có sử dụng engine hay không
        /// </summary>
        /// <returns></returns>
        bool IsUseFilterEngine { get; }

        /// <summary>
        /// Engine bộ lọc hiện tại
        /// </summary>
        /// <returns></returns>
        IFilterEngine CurrentEngine { get; }

    }
}
