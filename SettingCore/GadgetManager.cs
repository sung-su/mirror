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

        private IEnumerable<SettingGadgetInfo> installedGadgets;

        private GadgetManager()
        {
            // 1. get all installed gadgets for Settings
            installedGadgets = GadgetProvider.Gadgets.ToList();

            // 2. get current customization from file
            var fileCust = FileStorage.ReadFromFile(FileStorage.CurrentFilePath);
            UpdateGadgetsOrder(fileCust);

            // 4. save current customization to file
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFile(menuCustItems, FileStorage.CurrentFilePath);

            // 5. start file watching
            FileStorage.Instance.Changed += CustFileChanged;
            FileStorage.Instance.Lost += CustFileLost;
            FileStorage.Instance.StartMonitoring();
        }

        private void CustFileLost()
        {
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFile(menuCustItems, FileStorage.CurrentFilePath);
        }

        private void CustFileChanged()
        {
            var fileCust = FileStorage.ReadFromFile(FileStorage.CurrentFilePath);
            if (fileCust == null)
            {
                // file corrupted, so save current state
                Logger.Verbose("Cust file corrupted, saving file.");

                var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
                FileStorage.WriteToFile(menuCustItems, FileStorage.CurrentFilePath);
            }
            else
            {
                // update installed gadgets from file and trigger event to listeners
                Logger.Verbose("Cust file read, updating latest order from file and triggering event.");

                List<MenuCustomizationItem> changedItems = new List<MenuCustomizationItem>();
                foreach (var cust in fileCust)
                {
                    var gadget = installedGadgets.FirstOrDefault(x => x.Path.Equals(cust.MenuPath, StringComparison.InvariantCultureIgnoreCase));
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

        private void UpdateGadgetsOrder(IEnumerable<MenuCustomizationItem> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                var found = installedGadgets.Where(x => x.Path.Equals(item.MenuPath, StringComparison.InvariantCultureIgnoreCase));
                if (found.Count() == 1)
                {
                    found.First().Order = item.Order;
                }
            }
        }

        public void UpdateCustomization(string menuPath, int order)
        {
            var found = installedGadgets.Where(x => x.Path.Equals(menuPath, StringComparison.InvariantCultureIgnoreCase));
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

            // update file
            found.First().Order = order;
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFile(menuCustItems, FileStorage.CurrentFilePath);

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
            var info = installedGadgets.SingleOrDefault(x => x.Path.Equals(menuPath, StringComparison.InvariantCultureIgnoreCase));

            bool isMainMenu = info != null && info.IsMainMenu;
            Logger.Debug(menuPath + " is " + (isMainMenu ? "" : "NOT ") + "main menu");

            return isMainMenu;
        }

        internal bool DoesMenuPathMatchClassName(string menuPath, string fullClassName)
        {
            return installedGadgets.Where(x =>
            {
                bool menuPathStartsWith = menuPath.StartsWith(x.Path + ".", true, System.Globalization.CultureInfo.InvariantCulture);
                bool classNameEquals = x.ClassName.Equals(fullClassName, StringComparison.InvariantCultureIgnoreCase);
                return menuPathStartsWith && classNameEquals;
            }).Count() == 1;
        }

        internal IEnumerable<MenuCustomizationItem> GetCustomization(string fullClassName)
        {
            var menus = installedGadgets.Where(x => x.ClassName.Equals(fullClassName, StringComparison.InvariantCultureIgnoreCase));
            if (menus.Count() != 1)
            {
                Logger.Warn($"found {menus.Count()} gadgets for class: '{fullClassName}'");
                return new List<MenuCustomizationItem>();
            }

            string menuPath = menus.First().Path + ".";
            return installedGadgets
                .Where(x => x.Path.StartsWith(menuPath, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => new MenuCustomizationItem(x.Path, x.Order));
        }

        internal SettingGadgetInfo GetGadgetInfoFromPath(string menuPath)
        {
            var menus = installedGadgets.Where(x => x.Path.Equals(menuPath, StringComparison.InvariantCultureIgnoreCase));
            if (menus.Count() == 1)
            {
                return menus.First();
            }

            Logger.Warn($"found {menus.Count()} gadgets for menu path: '{menuPath}'");
            return null;
        }
    }
}
