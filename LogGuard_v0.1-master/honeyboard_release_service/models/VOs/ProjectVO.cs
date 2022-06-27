using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.models.VOs
{
    internal class ProjectVO
    {
        public string Name { get; set; } = "";
        public string PackageName { get; set; } = "";
        public string Path { get; set; } = "";
        public string VersionFilePath { get; set; } = "";

        public BranchVO? OnBranch { get; set; }
        public Dictionary<string, BranchVO> Branchs { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Path);
        }

        public ProjectVO(string projectPath)
        {
            Path = projectPath;
            Branchs = new Dictionary<string, BranchVO>();
        }

        public void AddProjectBranch(BranchVO bVO)
        {
            if (bVO.IsNode
                && !string.IsNullOrEmpty(bVO.BranchPath))
            {
                Branchs[bVO.BranchPath] = bVO;
            }
        }

        /// <summary>
        /// Thêm commit vVO vào tập hợp các commit của branch hiện tại
        /// Nếu như branch hiện tại đã tồn tại commit (thông qua cách check commit id)
        /// thì sẽ trả về commit đã tồn tại đó
        /// còn không sẽ trả về commit mới thêm vào
        /// </summary>
        /// <param name="vVO"></param>
        /// <returns></returns>
        public CommitVO? AddCommitVOToCurrentBranch(CommitVO vVO)
        {
            if (OnBranch == null
               || string.IsNullOrEmpty(OnBranch?.BranchPath)
               || OnBranch.CommitMap == null) return null;

            var versionDateMap = OnBranch.CommitMap;

            var listVersion = versionDateMap.ContainsKey(vVO.ReleaseDateTime.Date) ?
                versionDateMap[vVO.ReleaseDateTime.Date]
                : new List<CommitVO>();

            var existedVO = listVersion.Where(cm => cm.CommitId == vVO.CommitId).FirstOrDefault();
            if (existedVO == null)
            {
                existedVO = vVO;
                listVersion.Add(vVO);
            }

            if (!versionDateMap.ContainsKey(vVO.ReleaseDateTime.Date))
            {
                versionDateMap.Add(vVO.ReleaseDateTime.Date, listVersion);
            }

            return existedVO;
        }

        public void SetOnBranch(string branchPath)
        {
            if (Branchs.ContainsKey(branchPath))
            {
                OnBranch = Branchs[branchPath];
            }
        }

        public static ProjectVO GetTestData(string name, string package = "com.samsung.android")
        {
            var vo = new ProjectVO(@"C:/user/data/project/HoneyBoard")
            {
                OnBranch = new BranchVO(@"origin\master", "master", true, true),
                Name = name,
                PackageName = package,
                VersionFilePath = @"C:/user/data/project/HoneyBoard/version.properties",
            };
            vo.AddCommitVOToCurrentBranch(CommitVO.GetTestData("5.5.00.13", DateTime.Now.AddDays(-2)));
            vo.AddCommitVOToCurrentBranch(CommitVO.GetTestData("5.5.00.14", DateTime.Now));
            vo.AddCommitVOToCurrentBranch(CommitVO.GetTestData("5.5.00.15", DateTime.Now));
            vo.AddCommitVOToCurrentBranch(CommitVO.GetTestData("5.5.00.16", DateTime.Now));
            vo.AddCommitVOToCurrentBranch(CommitVO.GetTestData("5.5.00.17", DateTime.Now.AddDays(1)));

            return vo;
        }
    }
}
