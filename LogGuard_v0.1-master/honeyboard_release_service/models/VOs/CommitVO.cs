using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.models.VOs
{
    internal class CommitVO
    {
        public string CommitTitle { get; set; } = "";
        public string CommitId { get; set; } = "";
        public string AuthorEmail { get; set; } = "";
        public DateTime CommitDateTime { get; set; }
        
    }
}
