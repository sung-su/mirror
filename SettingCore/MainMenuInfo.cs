using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tizen.NUI;

namespace SettingCore
{
    public class MainMenuInfo
    {
        public string IconPath { get; set; }
        public string IconColorHex { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }

        private static string CachePath => System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Data, "main-menu.cache");
        public static List<MainMenuInfo> CacheMenu { get => cache; }
        private static List<MainMenuInfo> cache = new List<MainMenuInfo>();
        private const string metadataNamePrefix = "http://tizen.org/metadata/ui-gadget/menu";
        private const string iconPathMetadata = "icon-path";
        private const string iconColorMetadata = "icon-color";
        private const string titleMetadata = "title";
        static MainMenuInfo()
        {
            ReadCache();
        }

        private static void ReadCache()
        {
            try
            {
                string text = System.IO.File.ReadAllText(CachePath, Encoding.UTF8);
                string[] mainMenuItems = text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
                var fromCache = new List<MainMenuInfo>(mainMenuItems.Length);

                foreach (string menu in mainMenuItems)
                {
                    string[] keyValuePairs = menu.Split("\n");
                    var pairs = new Dictionary<string, string>(keyValuePairs.Length);
                    foreach (string keyValuePair in keyValuePairs)
                    {
                        var kv_split = keyValuePair.Split(':');
                        pairs[kv_split[0]] = kv_split[1];
                    }

                    fromCache.Add(new MainMenuInfo()
                    {
                        Title = pairs["Title"],
                        IconPath = pairs["IconPath"],
                        IconColorHex = pairs["IconColorHex"],
                        Path = pairs["Path"],
                    });
                }
                cache = fromCache;
            }
            catch
            {
                Logger.Warn($"Could not read main menu cache file.");
            }
        }

        public static void UpdateCache(IEnumerable<MainMenuInfo> infos)
        {
            if (infos == cache)
            {
                return;
            }
            try
            {
                cache.Clear();
                cache.AddRange(infos);

                string text = getCacheString();
                System.IO.File.WriteAllText(CachePath, text);
            }
            catch (Exception ex)
            {
                Logger.Warn($"{ex}");
            }
        }

        private static string getCacheString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MainMenuInfo info in cache)
            {
                sb.Append($"{getCacheString(info)}\n");
            }

            return sb.ToString();
        }

        private static string getCacheString(MainMenuInfo mainMenuInfo)
        {
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo prop in typeof(MainMenuInfo).GetProperties())
            {
                if (!prop.GetMethod.IsStatic)
                {
                    sb.Append($"{prop.Name}:{prop.GetValue(mainMenuInfo)}\n");
                }
            }
            return sb.ToString();
        }

        public static void ClearCache()
        {
            UpdateCache(new List<MainMenuInfo>());
        }

        private static MainMenuInfo FromManifest(SettingGadgetInfo info)
        {
            string iconPath = getResourcePath(info, getMetadata(info, $"{metadataNamePrefix}/{info.Path}/{iconPathMetadata}"));
            if (iconPath == null)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} manifest file.");
                return null;
            }

            string iconColor = getIconColorHex(info);
            if (iconColor == null)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} manifest file.");
                return null;
            }

            string title = getTitle(info);
            if (title == null)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} manifest file.");
                return null;
            }

            return new MainMenuInfo
            {
                IconPath = iconPath,
                IconColorHex = iconColor,
                Title = title,
                Path = info.Path,
            };
        }

        private static string getTitle(SettingGadgetInfo info)
        {
            var NUIGadgetResourceManager = new NUIGadgetResourceManager(info.Pkg);
            string titleMetadata = getMetadata(info, $"{metadataNamePrefix}/{info.Path}/{MainMenuInfo.titleMetadata}");
            string title = titleMetadata is null ? null : NUIGadgetResourceManager.GetString(titleMetadata);
            Logger.Verbose($"Get gadget title from manifest file: {titleMetadata} -> {title}");
            return string.IsNullOrEmpty(title) ? titleMetadata : title;
        }

        private static MainMenuInfo FromCache(SettingGadgetInfo info)
        {
            var cached = cache.SingleOrDefault(x => x.Path == info.Path);
            if (cached == null)
            {
                return null;
            }

            return new MainMenuInfo()
            {
                Title = cached.Title,
                IconPath = cached.IconPath,
                IconColorHex = cached.IconColorHex,
                Path = cached.Path,
            };
        }

        public static MainMenuInfo Create(SettingGadgetInfo info)
        {
            var menu = FromCache(info);
            if (menu != null)
            {
                Logger.Debug($"Loaded MainMenuInfo from Cache");
                return menu;
            }

            menu = FromManifest(info);
            if (menu != null)
            {
                Logger.Debug($"Loaded MainMenuInfo from Manifest file");
                return menu;
            }

            Logger.Debug($"Could NOT load MainMenuInfo, path: {info.Path}");
            return null;
        }

        private static string getIconColorHex(SettingGadgetInfo info)
        {
            string iconColorHex = getMetadata(info, $"{metadataNamePrefix}/{info.Path}/{iconColorMetadata}");
            if (iconColorHex is null)
            {
                return null;
            }
            var themeColors = iconColorHex.Split(",");
            if (themeColors.Length != 2)
            {
                return null;
            }
            bool IsLightTheme = ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";
            string iconColor = IsLightTheme ? themeColors[0] : themeColors[1];
            return iconColor;
        }

        private static string getMetadata(SettingGadgetInfo info, string metadataName)
        {
            if (info.Pkg.Metadata.TryGetValue(metadataName, out string data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }

        private static string getResourcePath(SettingGadgetInfo info, string relativeFilePath)
        {
            if (relativeFilePath is null)
                return null;
            string gadgetAssemplyName = info.Pkg.ExecutableFile.Replace(".dll", string.Empty);
            string absoluteDirPath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "mount/allowed/", gadgetAssemplyName);

            // remove leading slash
            relativeFilePath = relativeFilePath.TrimStart('/');

            return System.IO.Path.Combine(absoluteDirPath, relativeFilePath);
        }
    }
}
