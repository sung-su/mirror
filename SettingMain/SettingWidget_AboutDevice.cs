using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

namespace SettingMain
{
    class SettingWidget_AboutDevice : SettingWidget_Base
    {
        private static readonly ListItem[] listItems =
        {
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"sett_manage_cert_active_Default.png", "Manage certificate", ""),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"sett_legal_infot_active_Default.png", "Legal infomation", ""),
            new ListItem(1, "", "Device Info", ""),
            new ListItem(1, "", "Name", "Tizen"),
            new ListItem(1, "", "Model number", "rpi3"),
            new ListItem(1, "", "Tizen version", "TIZEN 6.5"),
            new ListItem(1, "", "CPU", "BCM2837"),
            new ListItem(1, "", "RAM", "4.0GB"),
            new ListItem(1, "", "Resolution", "1280 x 720"),
            new ListItem(1, "", "Status", "show network status and other infomation."),
        };

        public SettingWidget_AboutDevice()
            : base(listItems)
        {
        }
    }
}
