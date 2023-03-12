using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingCore.Customization
{
    internal class PreferenceStore : ICustomizationStore
    {
        private const string SettingMenuPathPrefix = "setting.menu.";

        public IEnumerable<string> MenuPaths => Tizen.Applications.Preference.Keys.Where(menuPath => menuPath.StartsWith(SettingMenuPathPrefix));

        public event EventHandler<CustomizationChangedEventArgs> Changed;

        protected void OnCustomizationChanged(CustomizationChangedEventArgs e)
        {
            var handler = Changed;
            handler?.Invoke(this, e);
        }

        public void Clear()
        {
            Logger.Debug("Clearing preference customization store.");
            foreach (var menuPath in MenuPaths)
            {
                Logger.Debug($"Removing menuPath: {menuPath}");
                Tizen.Applications.Preference.Remove(menuPath);
            }
            Logger.Debug("Cleared preference customization store.");
        }

        /// <summary>
        /// Sets order for menu path, if was not set yet. If menu path is set already, nothing happens.
        /// </summary>
        /// <param name="menuPath">Dot separated menu path.</param>
        /// <param name="order">Integer value, which indicated order on the menu list.</param>
        /// <returns>False, if menu path already exists and new order was not set. True, if menu path did not exists and order has been set.</returns>
        public bool SetOrder(string menuPath, int order)
        {
            menuPath = menuPath.ToLowerInvariant();

            if (MenuPaths.Contains(menuPath) )
            {
                Logger.Warn($"Cannot set order, because menuPath {menuPath} already exists. Please use UpdateOrder() or check menuPath.");
                return false;
            }

            Tizen.Applications.Preference.Set(menuPath, order);
            return true;
        }

        public void UpdateOrder(string menuPath, int order)
        {
            menuPath = menuPath.ToLowerInvariant();

            if (!MenuPaths.Contains(menuPath))
            {
                Logger.Warn($"Could not update order, because menuPath {menuPath} does not exists.");
                return;
            }

            Tizen.Applications.Preference.Set(menuPath, order);

            OnCustomizationChanged(new CustomizationChangedEventArgs(menuPath, order));
        }

        public int? GetOrder(string menuPath)
        {
            menuPath = menuPath.ToLowerInvariant();

            try
            {
                int order = Tizen.Applications.Preference.Get<int>(menuPath);
                return order;
            }
            catch (Exception exc)
            {
                Logger.Warn($"Could not get order from pref for {menuPath} ({exc})");
                return null;
            }
        }
    }
}
