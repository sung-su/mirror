using SettingCore.Customization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tizen.NUI;

namespace SettingCore
{
    public class GadgetManager
    {
        private const string SettingGadgetPackagePrefix = "org.tizen.setting";
        private const string GadgetClassSuffix = "gadget";

        private static ICustomizationStore customizationStore = new PreferenceStore();
        public static event EventHandler<CustomizationChangedEventArgs> CustomizationChanged;

        private static IEnumerable<SettingGadgetInfo> gadgets;

        static GadgetManager()
        {
            gadgets = getSettingGadgetInfos();

            // TODO: just for DEBUG, remove before merge
            customizationStore.Clear();

            // save order only if does not exists
            foreach (var gadget in gadgets)
            {
                customizationStore.SetOrder(gadget.Path, gadget.Order);
            }

            customizationStore.Changed += CustomizationStoreChanged;
        }

        private static void CustomizationStoreChanged(object sender, CustomizationChangedEventArgs e)
        {
            static bool equalsIgnoreCase(string a, string b) => a.ToLowerInvariant().Equals(b.ToLowerInvariant());

            // update order in gadgets collection
            var infos = gadgets.Where(g => equalsIgnoreCase(g.Path, e.MenuPath));
            if (infos.Count() != 1)
            {
                Logger.Warn($"cannot find gadget with menupath {e.MenuPath}");
                return;
            }
            Logger.Debug($"updating gadget order {e.Order} for menupath {e.MenuPath}");
            infos.First().Order = e.Order;

            // notifiy listeners
            var handler = CustomizationChanged;
            handler?.Invoke(sender, e);
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

        public static SettingGadgetInfo GetGadgetInfoFromPath(string menuPath)
        {
            var menus = gadgets.Where(x => x.Path == menuPath);
            if (menus.Count() == 1)
            {
                return menus.First();
            }

            Logger.Warn($"found {menus.Count()} gadgets for menu path: '{menuPath}'");
            return null;
        }

        public static IEnumerable<SettingGadgetInfo> GetMainWithCurrentOrder()
        {
            var main = gadgets
                .Where(info => info.IsMainMenu);

            Logger.Debug($"number of main gadgets: {main.Count()}");

            return main
                .OrderBy(info => info.Order);
        }

        public static void UpdateCustomization(string menuPath, int order)
        {
            // TODO: just for DEBUG, remove before merge
            Logger.Debug($"Customization BEFORE change:\n{customizationStore.CurrentCustomizationLog}");

            var found = gadgets.Where(x => x.Path == menuPath).Count();
            if (found == 0)
            {
                Logger.Warn($"Cannot update customization, because did not find gadget for menu path: {menuPath}.");
                return;
            }
            else if (found > 1)
            {
                Logger.Warn($"Cannot update customization, because 1+ gadgets found for menu path: {menuPath}.");
                return;
            }

            customizationStore.UpdateOrder(menuPath, order);

            // TODO: just for DEBUG, remove before merge
            Logger.Debug($"Customization AFTER change:\n{customizationStore.CurrentCustomizationLog}");
        }

        public static bool IsMainMenuPath(string menuPath)
        {
            var info = gadgets.SingleOrDefault(x => x.Path.ToLowerInvariant() == menuPath.ToLowerInvariant());

            bool isMainMenu = info != null && info.IsMainMenu;
            Logger.Debug(menuPath + " is " + (isMainMenu ? "" : "NOT ") + "main menu");

            return isMainMenu;
        }
    }
}
