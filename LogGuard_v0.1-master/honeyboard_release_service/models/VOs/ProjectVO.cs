using cyber_base.implement.utils;
using cyber_base.implement.views.cyber_treeview;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, SortedDictionary<DateTime, List<CommitVO>>> VersionMap { get; set; }
        public Dictionary<string, BranchVO> Branchs { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Path);
        }

        public ProjectVO(string projectPath)
        {
            Path = projectPath;
            VersionMap = new Dictionary<string, SortedDictionary<DateTime, List<CommitVO>>>();
            Branchs = new Dictionary<string, BranchVO>();
        }

        public void AddProjectBranch(BranchVO bVO)
        {
            if (bVO.IsNode
                && !string.IsNullOrEmpty(bVO.BranchPath))
            {
                Branchs[bVO.BranchPath] =  bVO;
            }
        }

        public void ClearCurrentBranchVersionMap()
        {
            if (string.IsNullOrEmpty(OnBranch?.BranchPath)) return;
            if (VersionMap.ContainsKey(OnBranch.BranchPath))
            {
                VersionMap[OnBranch.BranchPath].Clear();
            }
        }

        public void AddCurrentBranchVersionVO(CommitVO vVO)
        {
            if (string.IsNullOrEmpty(OnBranch?.BranchPath)) return;

            var versionDateMap = VersionMap.ContainsKey(OnBranch.BranchPath) ?
                VersionMap[OnBranch.BranchPath]
                : new SortedDictionary<DateTime, List<CommitVO>>();

            var listVersion = versionDateMap.ContainsKey(vVO.ReleaseDateTime.Date) ?
                versionDateMap[vVO.ReleaseDateTime.Date]
                : new List<CommitVO>();

            listVersion.Add(vVO);

            if (!versionDateMap.ContainsKey(vVO.ReleaseDateTime.Date))
            {
                versionDateMap.Add(vVO.ReleaseDateTime.Date, listVersion);
            }

            if (!VersionMap.ContainsKey(OnBranch.BranchPath))
            {
                VersionMap.Add(OnBranch.BranchPath, versionDateMap);
            }
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
            var list1 = new List<CommitVO>();
            var list2 = new List<CommitVO>();
            var list3 = new List<CommitVO>();
            list1.Add(CommitVO.GetTestData("5.5.00.13", DateTime.Now));
            list2.Add(CommitVO.GetTestData("5.5.00.14", DateTime.Now));
            list2.Add(CommitVO.GetTestData("5.5.00.15", DateTime.Now));
            list2.Add(CommitVO.GetTestData("5.5.00.16", DateTime.Now));
            list3.Add(CommitVO.GetTestData("5.5.00.17", DateTime.Now));

            var masterBranchVersionMap = new SortedDictionary<DateTime, List<CommitVO>>();
            masterBranchVersionMap.Add(DateTime.Today.AddDays(-2), list1);
            masterBranchVersionMap.Add(DateTime.Today, list2);
            masterBranchVersionMap.Add(DateTime.Today.AddDays(1), list3);
            vo.VersionMap.Add(vo.OnBranch.BranchPath, masterBranchVersionMap);
            return vo;
        }
    }
}
