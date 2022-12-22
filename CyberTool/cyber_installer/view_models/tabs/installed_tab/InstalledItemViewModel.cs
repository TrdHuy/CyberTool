using cyber_base.implement.command;
using cyber_base.ui_event_handler.action.executer;
using cyber_base.view_model;
using cyber_installer.@base.model;
using cyber_installer.definitions;
using cyber_installer.implement.modules.ui_event_handler;
using cyber_installer.implement.modules.utils;
using cyber_installer.model;
using cyber_installer.view.usercontrols.list_item.available_item.@base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace cyber_installer.view_models.tabs.installed_tab
{
    internal class InstalledItemViewModel : ItemViewModel
    {
        private ICommand _uninstallCommand;
        private BitmapImage? _iconCache;

        public ICommand UninstallCommand { get => _uninstallCommand; }

        public new BitmapImage? IconSource
        {
            get
            {
                return _iconCache;
            }
        }

        public InstalledItemViewModel(IToolInfo toolVO) : base(toolVO)
        {
            _uninstallCommand = new CommandExecuterImpl((paramaters) =>
            {
                var data = paramaters ?? this;
                return KeyActionListener.Current.OnKey(CyberInstallerDefinition.CYBER_INSTALLER_INDENTIFER
                    , CyberInstallerKeyFeatureTag.KEY_TAG_SWI_AT_UNISTALL_FEATURE
                    , data) as ICommandExecuter;
            }, isAsync: true);

        }

        protected async override void InstantiateItemStatus()
        {
            // Tạo riêng icon source load vào memory từ file
            // Mục đích: Trong lúc gỡ cài đặt phần mềm, sẽ xóa toàn bộ các file 
            // trong đó có file icon này, nếu sử dụng icon source là uri thông thường
            // sẽ không thể xóa file icon đó vì nó đang được sử dụng
            // Thay vào đó ta sẽ load nó vào trong memory và stream để hiển thị trực tiếp
            _iconCache = await Utils.CreateBitmapImageFromFile(_toolVO.IconSource);
            
            ItemStatus = ItemStatus.Installed;

            Invalidate("IconSource");
        }
    }
}
