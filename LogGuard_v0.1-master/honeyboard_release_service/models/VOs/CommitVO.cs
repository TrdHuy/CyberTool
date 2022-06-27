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
        public DateTime ReleaseDateTime { get; set; }
        public Uri? RetriverLink { get; set; }
        public Uri? QuickBuildLink { get; set; }
        public string AuthorEmail { get; set; } = "";
        public VersionPropertiesVO? Properties { get; set; }
        public string Version { get; set; } = "";

        public static CommitVO GetTestData(string version, DateTime releaseDate)
        {
            var vo = new CommitVO()
            {
                CommitTitle = version,
                CommitId = "a45b2c1",
                ReleaseDateTime = releaseDate,
                RetriverLink = new Uri("https://retriever.sw.sec.samsung.net"),
                QuickBuildLink = new Uri("https://package.qb.sec.samsung.net"),
            };
            return vo;
        }

        public static bool operator <(CommitVO t1, CommitVO t2)
        {

            return (t1.Properties ?? throw new NullReferenceException())
                < (t2.Properties ?? throw new NullReferenceException());
        }
        public static bool operator >(CommitVO t1, CommitVO t2)
        {
            return (t1.Properties ?? throw new NullReferenceException())
                > (t2.Properties ?? throw new NullReferenceException());
        }
        //public static bool operator ==(VersionVO t1, VersionVO t2)
        //{
        //    return (t1.Properties ?? throw new NullReferenceException())
        //        == (t2.Properties ?? throw new NullReferenceException());
        //}
        //public static bool operator !=(VersionVO t1, VersionVO t2)
        //{
        //    return (t1.Properties ?? throw new NullReferenceException())
        //        != (t2.Properties ?? throw new NullReferenceException());
        //}
        //public override bool Equals(object? obj)
        //{
        //    if (ReferenceEquals(this, obj))
        //    {
        //        return true;
        //    }

        //    if (ReferenceEquals(obj, null))
        //    {
        //        return false;
        //    }
        //    return (VersionVO)obj == this;
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
    }
}
