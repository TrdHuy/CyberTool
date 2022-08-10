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
        public string VersionAttrSyntax { get; set; } = "";

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
            var projectPathSplit = projectPath?.Split('\\');
            if (projectPathSplit?.Length > 1)
            {
                Name = projectPathSplit[projectPathSplit.Length - 1];
            }
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
        public VersionUpCommitVO? AddCommitVOToCurrentBranch(VersionUpCommitVO vVO)
        {
            if (OnBranch == null
               || string.IsNullOrEmpty(OnBranch?.BranchPath)
               || OnBranch.CommitMap == null) return null;

            var commitMap = OnBranch.CommitMap;

            if (!commitMap.ContainsKey(vVO.CommitId))
            {
                commitMap.Add(vVO.CommitId, vVO);
                return vVO;
            }
            else
            {
                return commitMap[vVO.CommitId];
            }
        }

        public void SetOnBranch(string branchPath)
        {
            if (Branchs.ContainsKey(branchPath))
            {
                OnBranch = Branchs[branchPath];
            }
        }
    }
}
