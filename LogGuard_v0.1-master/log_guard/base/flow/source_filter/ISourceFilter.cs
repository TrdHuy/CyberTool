using log_guard.@base.flow.highlight;

namespace log_guard.@base.flow.source_filter
{
    internal interface ISourceFilter : ISourceHighlightor
    {
        /// <summary>
        /// Lọc các giá trị theo điều kiện, trả về kiểu bool
        /// </summary>
        /// <param name="obj">đối tượng cần kiểm tra điều kiện để lọc</param>
        /// <returns></returns>
        bool Filter(object obj);
    }
}
