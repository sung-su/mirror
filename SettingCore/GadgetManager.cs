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
            // get all installed gadgets for Settings
            installedGadgets = GadgetProvider.Gadgets.ToList();

            // get initial customization from file
            var initCust = FileStorage.ReadFromFile(FileStorage.InitialFilePath);
            _ = UpdateCustomization(initCust);

            // get current customization from file
            var currentCust = FileStorage.ReadFromFile(FileStorage.CurrentFilePath);
            _ = UpdateCustomization(currentCust);

            // get backup customization from file (in case current was corrupted, when the app was shutdown)
            if (currentCust == null)
            {
                var backupCust = FileStorage.ReadFromFile(FileStorage.BackupFilePath);
                _ = UpdateCustomization(backupCust);
            }

            // save current customization to both files (current and backup)
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFiles(menuCustItems);

            // start file watching
            FileStorage.Instance.Changed += CustFileChanged;
            FileStorage.Instance.Lost += CustFileLost;
            FileStorage.Instance.StartMonitoring();
        }

        private void CustFileLost()
        {
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFiles(menuCustItems);
        }

        private void CustFileChanged()
        {
            var fileCust = FileStorage.ReadFromFile(FileStorage.CurrentFilePath);
            if (fileCust == null)
            {
                // file corrupted, so save current state
                Logger.Verbose("Cust file corrupted, saving file.");
            }
            else
            {
                // update installed gadgets from file and trigger event to listeners
                Logger.Verbose("Cust file read, updating latest order from file, triggering event and saving file.");

                var changedItems = UpdateCustomization(fileCust);
                if (changedItems == null || changedItems.Count() == 0)
                {
                    Logger.Verbose("None of customization items were changed.");
                    return;
                }

                // trigger event (with single or many changes)
                var handler = CustomizationChanged;
                handler?.Invoke(this, new CustomizationChangedEventArgs(changedItems));
            }

            // save current customization to both files (current and backup)
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFiles(menuCustItems);
        }

        /// <summary>
        /// Updates customization (order) for menu items, only when the order value changes.
        /// </summary>
        /// <param name="items">Collection of menu items with possibly new customization (order).</param>
        /// <returns>Collection of menu items which customization (order) has been updated, due to different value.</returns>
        private IEnumerable<MenuCustomizationItem> UpdateCustomization(IEnumerable<MenuCustomizationItem> items)
        {
            var updatedItems = new List<MenuCustomizationItem>();
            if (items == null)
            {
                return updatedItems;
            }

            Logger.Verbose($"Checking update for {items.Count()} menu items.");
            foreach (var item in items)
            {
                var found = installedGadgets.Where(x => x.Path.Equals(item.MenuPath, StringComparison.InvariantCultureIgnoreCase));
                if (found.Count() == 0)
                {
                    Logger.Warn($"Could not find menu item for menu path: {item.MenuPath}");
                    continue;
                }
                else if (found.Count() > 1)
                {
                    Logger.Warn($"Found 1+ menu items for menu path: {item.MenuPath}");
                    continue;
                }
                var foundItem = found.FirstOrDefault();
                if (foundItem == null)
                {
                    Logger.Warn($"Found menu item is null.");
                    continue;
                }

                if (foundItem.Order != item.Order)
                {
                    Logger.Verbose($"Updating menu path {item.MenuPath} with order {foundItem.Order} -> {item.Order}.");
                    foundItem.Order = item.Order;
                    updatedItems.Add(item);
                }
            }
            Logger.Verbose($"Updated {updatedItems.Count()} menu items.");

            return updatedItems;
        }

        public void ChangeMenuPathOrder(string menuPath, int order)
        {
            var items = new[] { new MenuCustomizationItem(menuPath, order) };
            var changedItems = UpdateCustomization(items);
            if (changedItems == null || changedItems.Count() == 0)
            {
                Logger.Verbose("None of customization items were changed.");
                return;
            }

            // trigger event (with single change)
            var handler = CustomizationChanged;
            handler?.Invoke(this, new CustomizationChangedEventArgs(menuPath, order));

            // save current customization to both files (current and backup)
            var menuCustItems = installedGadgets.Select(x => new MenuCustomizationItem(x.Path, x.Order));
            FileStorage.WriteToFiles(menuCustItems);
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
