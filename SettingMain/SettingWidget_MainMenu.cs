using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

namespace SettingMain
{
    class SettingWidget_MainMenu : SettingWidget_Base
    {
        private static readonly ListItem[] mItems =  {
            // Connections
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_wifi.png", "Wifi","Wifi"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_bluetooth.png", "Bluetooth","Bluetooth"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_wifi.png", "Wired Network","Wired Network"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_wifi.png", "Tethering","Tethering"),
            
            // Sound
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_sound_and_notifications.png", "Sound","Sound"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_notifications.png", "Notifications","Notifications"),

            // Display
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_display.png", "Display","Display"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_wallpapers.png", "Wallpaper","Wallpaper"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_softkey.png", "Tray","Tray"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_softkey.png", "Screen Mirroring","Screen Mirroring"),

            // Personal
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_account.png", "Accounts","Accounts"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_privacy_and_safety.png", "Privacy","Privacy"),

            // Memory
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_applications.png", "Apps","Applications"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_storage.png", "Storage","Storage"),

            // System
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_language_and_input.png", "Language and input","Language and input"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_date_and_time.png", "Date and time","Date and time"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_reset.png", "Reset","Reset"),
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_accessibility.png", "Accessibility","Accessibility"),

            // Device
            new ListItem(1, SETTING_LIST_ICON_PATH_CFG+"settings_about_device.png", "About device","About device"),
        };

        public SettingWidget_MainMenu()
            :base(mItems)
        { 
        }
    }

}
