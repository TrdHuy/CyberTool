using cyber_base.implement.attributes;
using log_guard.@base.flow.source_filter;

namespace log_guard.@base.flow.source_filterr
{
    internal enum FilterType
    {
        [StringValue("Simple")]
        Simple = 0,

        [StringValue("Syntax")]
        Syntax = 1,

        [StringValue("Advance")]
        Advance = 2
    }

    internal interface IMechanicalSourceFilter : ISourceFilter
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
