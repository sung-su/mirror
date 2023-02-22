using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;

namespace SettingCore
{
    public class GadgetManager
    {
        private const string SettingGadgetPackagePrefix = "org.tizen.setting";
        private const string SettingMenuMetadataPrefix = "setting/menu/";

        // dict key is metadata key from tizen-manifest.xml file (e.g. setting/menu/wifi)
        private static Dictionary<string, SettingGadgetInfo> gadgets;

        static GadgetManager()
        {
            gadgets = getSettingGadgets();
        }

        private static Dictionary<string, SettingGadgetInfo> getSettingGadgets()
        {
            Dictionary<string, SettingGadgetInfo> dict = new Dictionary<string, SettingGadgetInfo>();

            var allGadgetPackages = NUIGadgetManager.GetGadgetInfos();
            var settingGadgetPackages = allGadgetPackages.Where(pkg => pkg.PackageId.StartsWith(SettingGadgetPackagePrefix));

            Logger.Debug($"all gadget packages: {allGadgetPackages.Count()}, setting gadget packages: {settingGadgetPackages.Count()}");
            foreach (var pkg in settingGadgetPackages)
            {
                Logger.Debug($"setting gadget package (pkgId: {pkg.PackageId}, resType: {pkg.ResourceType})");
            }
            foreach (var pkg in allGadgetPackages.Where(p => !settingGadgetPackages.Contains(p)))
            {
                Logger.Debug($"other gadget package (pkgId: {pkg.PackageId}, resType: {pkg.ResourceType})");
            }

            foreach (var pkg in settingGadgetPackages)
            {
                var metadatas = pkg.Metadata.Where(pair => pair.Key.StartsWith(SettingMenuMetadataPrefix));
                foreach (var metadata in metadatas)
                {
                    if (!int.TryParse(metadata.Value, out int orderId))
                    {
                        Logger.Warn($"metadata value (orderId) is not integer (pkgId: {pkg.PackageId}, resType: {pkg.ResourceType}, key:{metadata.Key}, value:{metadata.Value})");
                        continue;
                    }

                    var parts = metadata.Key.Split('/');
                    var partsCapitalized = parts.Select(s => s[..1].ToUpperInvariant() + s[1..]);
                    string className = string.Join('.', partsCapitalized) + "Gadget";

                    bool isMainMenu = parts.Length == 3; // depends on SettingMenuMetadataPrefix.Length

                    var settingGadgetInfo = new SettingGadgetInfo(pkg, orderId, className, isMainMenu);
                    Logger.Debug($"found gadget ({settingGadgetInfo})");

                    dict.Add(metadata.Key, settingGadgetInfo);
                }
            }

            return dict;
        }

        public static IEnumerable<SettingGadgetInfo> GetAll()
        {
            return gadgets
                .Select(pair => pair.Value);
        }

        public static IEnumerable<SettingGadgetInfo> GetMain()
        {
            return gadgets
                .Select(pair => pair.Value)
                .Where(info => info.IsMainMenu)
                .OrderBy(info => info.OrderId);
        }

        public static Dictionary<string, int> GetDefaultCustomization()
        {
            var customization = gadgets
                .Select(pair => new KeyValuePair<string, int>(pair.Key, pair.Value.OrderId));

            return new Dictionary<string, int>(customization);
        }
    }
}
