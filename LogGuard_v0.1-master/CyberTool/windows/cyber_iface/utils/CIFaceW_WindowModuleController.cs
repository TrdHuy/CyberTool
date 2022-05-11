using cyber_tool.windows.cyber_iface.pages.page_controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_tool.windows.cyber_iface.utils
{
    public class CIFaceW_WindowModuleController
    {
        private static object? _CIFW_PC_Instance;

        public static CIFaceW_PageController CIFW_PC_Instance
        {
            get
            {
                if (_CIFW_PC_Instance == null)
                {
                    _CIFW_PC_Instance = Activator.CreateInstance(typeof(CIFaceW_PageController), true);
                }
                return _CIFW_PC_Instance as CIFaceW_PageController;
            }
        }

    }
}
