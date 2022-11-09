using cyber_base.view_model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_installer.view_models.windows
{
    internal class ChooseInstallLocationViewModel : BaseViewModel
    {
        [Bindable(true)]
        public string SpaceRequire
        {
            get
            {
                return "";
            }
        }

        [Bindable(true)]
        public string SoftwareName
        {
            get
            {
                return "Cyber tool";
            }
        }

        [Bindable(true)]
        public string ImageSource
        {
            get
            {
                return "https://cdn.icon-icons.com/icons2/3191/PNG/512/cyclone_weather_world_time_icon_194253.png";
            }
        }
    }
}
