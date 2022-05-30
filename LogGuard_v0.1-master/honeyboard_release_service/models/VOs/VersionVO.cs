using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.models.VOs
{
    internal class VersionVO
    {
        public string Name { get; set; } = "";
        public string CommitId { get; set; } = "";
        public DateTime ReleaseDateTime { get; set; }
        public Uri? RetriverLink { get; set; }
        public Uri? QuickBuildLink { get; set; }

        public static VersionVO GetTestData(string version, DateTime releaseDate)
        {
            var vo = new VersionVO()
            {
                Name = version,
                CommitId = "a45b2c1",
                ReleaseDateTime = releaseDate,
                RetriverLink = new Uri("https://retriever.sw.sec.samsung.net"),
                QuickBuildLink = new Uri("https://package.qb.sec.samsung.net"),
            };
            return vo;
        }
    }
}
