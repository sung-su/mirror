using SettingCore.Customization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SettingCore
{
    public class GadgetManager
    {
        private static GadgetManager instance = new GadgetManager();
        public static GadgetManager Instance => instance;

        public event EventHandler<CustomizationChangedEventArgs> CustomizationChanged;

        private ICustomizationStore customizationStore;
        private IEnumerable<SettingGadgetInfo> installedGadgets;

        private GadgetManager()
        {
            customizationStore = new PreferenceStore();

            // 1. get all installed gadgets for Settings
            installedGadgets = GadgetProvider.Gadgets.ToList();

            // 2. get file customization
            var fileCust = FileStorage.ReadFromFile();
            if (fileCust == null)
            {
                // no file, so get current/latest orderfrom pref
                Logger.Verbose("No cust file, updating latest order from Preferences.");

                foreach (var gadget in installedGadgets)
                {
                    string gadgetMenuPath = gadget.Path.ToLowerInvariant();
                    int? order = customizationStore.GetOrder(gadgetMenuPath);
                    if (order.HasValue)
                    {
                        gadget.Order = order.Value;
                    }
                }
            }
            else
            {
                // file exists, so get current/latest order from file
                Logger.Verbose("Cust file read, updating latest order from file.");

                foreach (var gadget in installedGadgets)
                {
                    string gadgetMenuPath = gadget.Path.ToLowerInvariant();
                    var item = fileCust.FirstOrDefault(x => x.MenuPath == gadgetMenuPath);
                    if (item != null)
                    {
                        gadget.Order = item.Order;
                    }
                }
            }

            // 3. update to preferences
            foreach (var gadget in installedGadgets)
            {
                string gadgetMenuPath = gadget.Path.ToLowerInvariant();
                int? order = customizationStore.GetOrder(gadgetMenuPath);
                if (order.HasValue)
                {
                    customizationStore.UpdateOrder(gadgetMenuPath, gadget.Order);
                }
                else
                {
                    customizationStore.SetOrder(gadgetMenuPath, gadget.Order);
                }
            }

            // TODO: remove entry from Preferences if no gadget installed anymore

            // 4. update to JSON file
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFile(menuCustItems);

            // 5. start file watching
            FileStorage.Instance.Changed += CustFileChanged;
            FileStorage.Instance.Lost += CustFileLost;
            FileStorage.Instance.StartMonitoring();
        }

        private void CustFileLost()
        {
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFile(menuCustItems);
        }

        private void CustFileChanged()
        {
            var fileCust = FileStorage.ReadFromFile();
            if (fileCust == null)
            {
                // file corrupted, so save current state
                Logger.Verbose("Cust file corrupted, saving file.");

                var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
                FileStorage.WriteToFile(menuCustItems);
            }
            else
            {
                // update installed gadgets from file and trigger event to listeners
                Logger.Verbose("Cust file read, updating latest order from file and triggering event.");

                List<MenuCustomizationItem> changedItems = new List<MenuCustomizationItem>();
                foreach (var cust in fileCust)
                {
                    var gadget = installedGadgets.FirstOrDefault(x => x.Path.ToLowerInvariant() == cust.MenuPath.ToLowerInvariant());
                    if (gadget != null && gadget.Order != cust.Order)
                    {
                        gadget.Order = cust.Order;
                        changedItems.Add(cust);
                    }
                }

                // trigger event (with single or many changes)
                var handler = CustomizationChanged;
                handler?.Invoke(this, new CustomizationChangedEventArgs(changedItems));
            }
        }

        public void UpdateCustomization(string menuPath, int order)
        {
            var found = installedGadgets.Where(x => x.Path.ToLowerInvariant() == menuPath.ToLowerInvariant());
            if (found.Count() == 0)
            {
                Logger.Warn($"Cannot update customization, because did not find gadget for menu path: {menuPath}.");
                return;
            }
            else if (found.Count() > 1)
            {
                Logger.Warn($"Cannot update customization, because 1+ gadgets found for menu path: {menuPath}.");
                return;
            }

            // update pref
            customizationStore.UpdateOrder(menuPath, order);

            // update file
            found.First().Order = order;
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFile(menuCustItems);

            // trigger event (with single change)
            var handler = CustomizationChanged;
            handler?.Invoke(this, new CustomizationChangedEventArgs(menuPath, order));
        }

        public IEnumerable<SettingGadgetInfo> GetMainWithCurrentOrder()
        {
            var main = installedGadgets
                .Where(info => info.IsMainMenu);

            Logger.Debug($"number of main gadgets: {main.Count()}");

            return main
                .OrderBy(info => info.Order);
        }

        public bool IsMainMenuPath(string menuPath)
        {
            var info = installedGadgets.SingleOrDefault(x => x.Path.ToLowerInvariant() == menuPath.ToLowerInvariant());

            bool isMainMenu = info != null && info.IsMainMenu;
            Logger.Debug(menuPath + " is " + (isMainMenu ? "" : "NOT ") + "main menu");

            return isMainMenu;
        }

        internal bool DoesMenuPathMatchClassName(string menuPath, string fullClassName)
        {
            return installedGadgets.Where(x =>
            {
                bool menuPathStartsWith = menuPath.StartsWithIgnoreCase(x.Path + ".");
                bool classNameEquals = x.ClassName.EqualsIgnoreCase(fullClassName);
                return menuPathStartsWith && classNameEquals;
            }).Count() == 1;
        }

        internal IEnumerable<MenuCustomizationItem> GetCustomization(string fullClassName)
        {
            var menus = installedGadgets.Where(x => x.ClassName.EqualsIgnoreCase(fullClassName));
            if (menus.Count() != 1)
            {
                Logger.Warn($"found {menus.Count()} gadgets for class: '{fullClassName}'");
                return new List<MenuCustomizationItem>();
            }

            string menuPath = menus.First().Path + ".";
            return installedGadgets
                .Where(x => x.Path.StartsWithIgnoreCase(menuPath))
                .Select(x => new MenuCustomizationItem(x.Path, x.Order));
        }

        internal SettingGadgetInfo GetGadgetInfoFromPath(string menuPath)
        {
            var menus = installedGadgets.Where(x => x.Path == menuPath);
            if (menus.Count() == 1)
            {
                return menus.First();
            }

            Logger.Warn($"found {menus.Count()} gadgets for menu path: '{menuPath}'");
            return null;
        }
    }
}
