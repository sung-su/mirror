using SettingCore.TextResources;
using SettingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;

namespace SettingMainGadget.Display
{
    public class DisplayThemeManager
    {
        public class ThemeInfo
        {
            private string Name;
            private string Id;

            public ThemeInfo(string name, string id)
            {
                Name = name;
                Id = id;
            }

            public string GetName()
            {
                return Name;
            }

            public string GetId()
            {
                return Id;
            }
        };

        public static readonly List<ThemeInfo> ThemeList = new List<ThemeInfo>
        {
            new ThemeInfo("Light theme", "org.tizen.default-light-theme"),
            new ThemeInfo("Dark theme", "org.tizen.default-dark-theme"),
        }; // TODO : add name of the theme to the Resources so it can be translated when changing the language

        public static void SetTheme(string Id)
        {
            try
            {
                ThemeManager.ApplyPlatformTheme(Id);
            }
            catch (Exception ex)
            {
                Logger.Error($"({Id}): {ex.Message}");
            }
        }

        public static int GetThemeIndex()
        {
            string themeId = ThemeManager.PlatformThemeId;

            if (string.IsNullOrEmpty(themeId))
            {
                Logger.Warn("Theme : Not Available");
                return -1;
            }

            Logger.Debug($"Theme : {themeId}");

            var theme = ThemeList.Where(x => x.GetId().Equals(themeId)).First();

            if(theme != null)
            {
                return ThemeList.IndexOf(theme); 
            }

            return -1;
        }

        public static string GetThemeName()
        {
            int index = GetThemeIndex();

            return index >= 0 ? ThemeList[index].GetName() : Resources.IDS_ST_HEADER_UNAVAILABLE;
        }
    }
}
