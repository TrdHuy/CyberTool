using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow
{
    public interface ISourceHighlightor
    {
        /// <summary>
        /// Highlight các giá trị theo điều kiện, trả về kiểu bool
        /// </summary>
        /// <param name="obj">đối tượng cần highlight</param>
        /// <returns>true: nếu có chuỗi đã được highlight</returns>
        /// <returns>false: nếu không có chuỗi nào được highlight</returns>
        bool Highlight(object obj);

        /// <summary>
        /// Bỏ toàn bộ các đối tượng đang highlight trong 1 chuỗi
        /// </summary>
        /// <param name="obj">đối tượng cần bỏ highlight</param>
        void Clean(object obj);
    }
}
