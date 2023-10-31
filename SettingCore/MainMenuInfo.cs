using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Tizen.NUI;

namespace SettingCore
{
    public class MainMenuInfo
    {
        public string IconPath { get; set; }
        public Color IconColor { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }

        private static string CachePath => System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Data, "main-menu.cache");
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
                string text = System.IO.File.ReadAllText(CachePath);
                cache = JsonSerializer.Deserialize<List<MainMenuInfo>>(text);
            }
            catch
            {
                Logger.Warn($"Could not read main menu cache file.");
            }
        }

        public static void UpdateCache(IEnumerable<MainMenuInfo> infos)
        {
            try
            {
                cache.Clear();
                cache.AddRange(infos);
                string text = JsonSerializer.Serialize(cache);
                System.IO.File.WriteAllText(CachePath, text);
            }
            catch (Exception ex)
            {
                Logger.Warn($"{ex}");
            }
        }

        public static void ClearCache()
        {
            UpdateCache(new List<MainMenuInfo>());
        }

        private static MainMenuInfo FromGadget(SettingGadgetInfo info)
        {
            string assemblyPath = System.IO.Path.Combine(info.Pkg.ResourcePath, info.Pkg.ExecutableFile);
            System.Reflection.Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(System.IO.File.ReadAllBytes(assemblyPath));
            }
            catch (System.IO.FileLoadException)
            {
                Logger.Warn($"could not open assembly {assemblyPath}");
                return null;
            }
            MainMenuGadget mainMenu;
            try
            {
                mainMenu = NUIGadgetManager.Add(info.Pkg.ResourceType, info.ClassName) as MainMenuGadget;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} at {assemblyPath}");
                Logger.Error(e.Message);
                return null;
            }
            if (mainMenu == null)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} at {assemblyPath}");
                return null;
            }

            string iconPath = mainMenu.ProvideIconPath();
            Color iconColor = mainMenu.ProvideIconColor();
            string title = mainMenu.ProvideTitle();

            NUIGadgetManager.Remove(mainMenu);

            return new MainMenuInfo
            {
                IconPath = iconPath,
                IconColor = iconColor,
                Title = title,
                Path = info.Path,
            };
        }

        private static MainMenuInfo FromManifest(SettingGadgetInfo info)
        {
            string iconPath = getResourcePath(info, getMetadata(info, $"{metadataNamePrefix}/{info.Path}/{iconPathMetadata}"));
            if (iconPath == null)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} manifest file.");
                return null;
            }

            Color iconColor = getIconColor(info);
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
                IconColor = iconColor,
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
                IconColor = cached.IconColor,
                Path = cached.Path,
            };
        }

        public static MainMenuInfo Create(SettingGadgetInfo info)
        {
            TimeSpan total = TimeSpan.Zero;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            var menu = FromCache(info);
            stopwatch.Stop();
            total += stopwatch.Elapsed;
            if (menu != null)
            {
                Logger.Debug($"MEASURE loaded MainMenuInfo from Cache, path: {info.Path}, time: {stopwatch.Elapsed}");
                return menu;
            }

            stopwatch.Restart();
            menu = FromManifest(info);
            stopwatch.Stop();
            total += stopwatch.Elapsed;
            if (menu != null)
            {
                Logger.Debug($"MEASURE loaded MainMenuInfo from Manifest file, path: {info.Path}, time: {stopwatch.Elapsed}");
                return menu;
            }

            stopwatch.Restart();
            menu = FromGadget(info);
            stopwatch.Stop();
            total += stopwatch.Elapsed;
            if (menu != null)
            {
                Logger.Debug($"MEASURE loaded MainMenuInfo from Gadget, path: {info.Path}, time: {stopwatch.Elapsed}");
                return menu;
            }

            Logger.Debug($"MEASURE could NOT load MainMenuInfo, path: {info.Path}, time: {total}");
            return null;
        }

        private static Color getIconColor(SettingGadgetInfo info)
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
            Color iconColor = iconColorHex is null ? null : new Color(IsLightTheme ? themeColors[0] : themeColors[1]); ;
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
