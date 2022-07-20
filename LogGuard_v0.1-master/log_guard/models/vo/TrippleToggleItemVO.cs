using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log_guard.models.vo
{
    internal class TrippleToggleItemVO
    {
        public string Content { get; set; }
        public Status Stat { get; set; }

        public TrippleToggleItemVO(string content)
        {
            Content = content;
            Stat = Status.None;
        }

        public enum Status
        {
            None = 0,
            Show = 1,
            Remove = 2
        }

    }
}
