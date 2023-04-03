using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tizen.NUI;

namespace SettingCore
{
    internal class GadgetProvider
    {
        private static readonly string[] SettingGadgetPackagePrefixes = new string[]
        {
            "org.tizen.setting",
            "org.tizen.cssetting",
        };

        public static IEnumerable<SettingGadgetInfo> Gadgets => getSettingGadgetInfos();

        private static IEnumerable<SettingGadgetInfo> getSettingGadgetInfos()
        {
            var allGadgetPackages = NUIGadgetManager.GetGadgetInfos();

            static bool StartsWithAny(string packageId, IEnumerable<string> prefixes)
            {
                foreach (var prefix in prefixes)
                {
                    if (packageId.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
                return false;
            }
            var settingGadgetPackages = allGadgetPackages.Where(pkg => StartsWithAny(pkg.PackageId, SettingGadgetPackagePrefixes));

            Logger.Debug($"all gadget packages: {allGadgetPackages.Count()}, setting gadget packages: {settingGadgetPackages.Count()}");
            foreach (var pkg in settingGadgetPackages)
                Logger.Debug($"setting gadget package (pkgId: {pkg.PackageId}, resType: {pkg.ResourceType})");
            foreach (var pkg in allGadgetPackages.Where(p => !settingGadgetPackages.Contains(p)))
                Logger.Debug($"other gadget package (pkgId: {pkg.PackageId}, resType: {pkg.ResourceType})");

            List<SettingGadgetInfo> collection = new List<SettingGadgetInfo>();

            foreach (var gadgetInfo in settingGadgetPackages)
            {
                var settingGadgetInfos = getSettingGadgetInfos(gadgetInfo);
                collection.AddRange(settingGadgetInfos);
            }

            // check is there is more than one gadget with the same Menu Path
            var uniqueMenuPaths = collection.Select(i => i.Path).Distinct();
            foreach (var menuPath in uniqueMenuPaths)
            {
                var found = collection.Where(i => i.Path == menuPath);
                if (found.Count() > 1)
                {
                    Logger.Warn($"Customization may work INCORRECTLY due to the same menu path '{menuPath}' for {found.Count()} gadgets.");
                    foreach (var info in found)
                        Logger.Warn($"Menu path: {menuPath} -> {info.ClassName} @ {info.Pkg.PackageId}");
                }
            }

            return collection;
        }

        private static IEnumerable<SettingGadgetInfo> getSettingGadgetInfos(NUIGadgetInfo gadgetInfo)
        {
            string assemblyPath = System.IO.Path.Combine(gadgetInfo.ResourcePath, gadgetInfo.ExecutableFile);
            Assembly assembly = null;
            try
            {
                Logger.Verbose($"Opening assembly from {assemblyPath} ({gadgetInfo.ResourcePath}, {gadgetInfo.ExecutableFile})");
                assembly = Assembly.Load(System.IO.File.ReadAllBytes(assemblyPath));
            }
            catch (System.IO.FileLoadException)
            {
                Logger.Warn($"could not load assembly {assemblyPath}");
                yield break;
            }
            catch (System.IO.FileNotFoundException)
            {
                Logger.Warn($"could not find assembly {assemblyPath}");
                yield break;
            }

            var providerType = assembly.GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(SettingMenuProvider)))
                .SingleOrDefault();
            if (providerType == null)
            {
                Logger.Warn($"could not find setting menu provider at {assemblyPath}");
                yield break;
            }

            var settingMenuProvider = assembly.CreateInstance(providerType.FullName) as SettingMenuProvider;
            var settingMenus = settingMenuProvider.Provide();
            Logger.Debug($"Provider {providerType.FullName} contains {settingMenus.Count()} menus");

            foreach (var settingMenu in settingMenus)
            {
                Logger.Verbose($"{settingMenu}");
                yield return new SettingGadgetInfo(gadgetInfo, settingMenu);
            }
        }
    }
}
