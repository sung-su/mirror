using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tizen.NUI;

namespace SettingCore
{
    public class GadgetManager
    {
        private const string SettingGadgetPackagePrefix = "org.tizen.setting";
        private const string SettingMenuMetadataPrefix = "setting.menu.";
        private const string GadgetClassSuffix = "gadget";

        private static IEnumerable<SettingGadgetInfo> gadgets;

        static GadgetManager()
        {
            gadgets = getSettingGadgetInfos();
        }

        private static IEnumerable<SettingGadgetInfo> getSettingGadgetInfos()
        {
            var allGadgetPackages = NUIGadgetManager.GetGadgetInfos();
            var settingGadgetPackages = allGadgetPackages.Where(pkg => pkg.PackageId.StartsWith(SettingGadgetPackagePrefix));

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

            return collection;
        }

        private static IEnumerable<SettingGadgetInfo> getSettingGadgetInfos(NUIGadgetInfo gadgetInfo)
        {
            string assemblyPath = System.IO.Path.Combine(gadgetInfo.ResourcePath, gadgetInfo.ExecutableFile);
            System.Reflection.Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(System.IO.File.ReadAllBytes(assemblyPath));
            }
            catch (System.IO.FileLoadException)
            {
                Logger.Warn($"could not open assembly {assemblyPath}");
                yield break;
            }

            var providerType = assembly.GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(SettingCore.SettingMenuProvider)))
                .SingleOrDefault();
            if (providerType == null)
            {
                Logger.Warn($"could not find setting menu provider at {assemblyPath}");
                yield break;
            }

            Logger.Debug($"provider type: {providerType.FullName}");
            var settingMenuProvider = assembly.CreateInstance(providerType.FullName) as SettingCore.SettingMenuProvider;
            var settingMenus = settingMenuProvider.Provide();
            Logger.Debug($"{providerType} contains {settingMenus.Count()} menus");

            foreach (var settingMenu in settingMenus)
            {
                Logger.Verbose($"{settingMenu}");
                yield return new SettingGadgetInfo(gadgetInfo, settingMenu);
            }
        }

        public static IEnumerable<SettingGadgetInfo> GetAll() => gadgets.ToList();

        public static IEnumerable<SettingGadgetInfo> GetMainWithDefaultOrder()
        {
            var main = gadgets
                .Where(info => info.IsMainMenu);

            Logger.Debug($"number of main gadgets: {main.Count()}");

            return main
                .OrderBy(info => info.Order);
        }
    }
}
