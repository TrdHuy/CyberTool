using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.models.VOs
{
    internal class CommitVO
    {
        public string CommitTitle { get; set; } = "";
        public string CommitId { get; set; } = "";
        public string AuthorEmail { get; set; } = "";
        public DateTime CommitDateTime { get; set; }
        
    }
}
