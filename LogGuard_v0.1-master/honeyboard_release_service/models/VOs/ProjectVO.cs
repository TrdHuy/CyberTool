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
        public Dictionary<DateTime, List<VersionVO>>? VersionMap { get; set; }

        public static ProjectVO GetTestData(string name, string package = "com.samsung.android")
        {
            var vo = new ProjectVO()
            {
                Name = name,
                PackageName = package,
                Path = @"C:/user/data/project/HoneyBoard",
                VersionFilePath = @"C:/user/data/project/HoneyBoard/version.properties",
                VersionMap = new Dictionary<DateTime, List<VersionVO>>()
            };
            var list1 = new List<VersionVO>();
            var list2 = new List<VersionVO>();
            var list3 = new List<VersionVO>();
            list1.Add(VersionVO.GetTestData("5.5.00.13", DateTime.Now));
            list2.Add(VersionVO.GetTestData("5.5.00.14", DateTime.Now));
            list2.Add(VersionVO.GetTestData("5.5.00.15", DateTime.Now));
            list2.Add(VersionVO.GetTestData("5.5.00.16", DateTime.Now));
            list3.Add(VersionVO.GetTestData("5.5.00.17", DateTime.Now));

            vo.VersionMap.Add(DateTime.Today.AddDays(-2), list1);
            vo.VersionMap.Add(DateTime.Today, list2);
            vo.VersionMap.Add(DateTime.Today.AddDays(1), list3);

            return vo;
        }
    }
}
