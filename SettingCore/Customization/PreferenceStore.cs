using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SettingCore.Customization
{
    internal class PreferenceStore : ICustomizationStore
    {
        private const string SettingMenuPathPrefix = "setting.menu.";

        private IEnumerable<string> menuPaths => Tizen.Applications.Preference.Keys.Where(menuPath => menuPath.StartsWith(SettingMenuPathPrefix));

        public string CurrentCustomizationLog
        {
            get
            {
                var dict = new Dictionary<string, int>();
                foreach (var menuPath in menuPaths)
                {
                    try
                    {
                        int order = GetOrder(menuPath);
                        dict.Add(menuPath, order);
                    }
                    catch
                    {
                        Logger.Warn($"Could not find order for menuPath {menuPath}.");
                    }
                }

                StringBuilder sb = new StringBuilder();
                foreach (var pair in dict.OrderBy(kv => Math.Abs(kv.Value)))
                {
                    sb.AppendLine($" ****** Menu Path: {pair.Key}, Order: {pair.Value}");
                }
                return sb.ToString();
            }
        }

        public event EventHandler<CustomizationChangedEventArgs> Changed;

        protected void OnCustomizationChanged(CustomizationChangedEventArgs e)
        {
            var handler = Changed;
            handler?.Invoke(this, e);
        }

        public void Clear()
        {
            Logger.Debug("Clearing preference customization store.");
            foreach (var menuPath in menuPaths)
            {
                Logger.Debug($"Removing menuPath: {menuPath}");
                Tizen.Applications.Preference.Remove(menuPath);
            }
            Logger.Debug("Cleared preference customization store.");
        }

        public bool SetOrder(string menuPath, int order)
        {
            menuPath = menuPath.ToLowerInvariant();

            if (menuPaths.Contains(menuPath) )
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

            if (!menuPaths.Contains(menuPath))
            {
                Logger.Warn($"Could not update order, because menuPath {menuPath} does not exists.");
                return;
            }

            Tizen.Applications.Preference.Set(menuPath, order);

            OnCustomizationChanged(new CustomizationChangedEventArgs(menuPath, order));
        }

        public int GetOrder(string menuPath)
        {
            menuPath = menuPath.ToLowerInvariant();

            int order = Tizen.Applications.Preference.Get<int>(menuPath);
            return order;
        }
    }
}
