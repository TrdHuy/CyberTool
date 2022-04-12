using LogGuard_v0._1.AppResources.AttachedProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogGuard_v0._1.Base.LogGuardFlow
{
    public interface ISourceFilter : ISourceHighlightor
    {
        /// <summary>
        /// Lọc các giá trị theo điều kiện, trả về kiểu bool
        /// </summary>
        /// <param name="obj">đối tượng cần kiểm tra điều kiện để lọc</param>
        /// <returns></returns>
        bool Filter(object obj);

        void UpdateEngine();

        void UpdateEngingeComparableSource(string source);
    }
}
