using System.Linq;

namespace SettingCore.Customization
{
    public class MenuCustomizationItem
    {
        /// <summary>
        /// full key (e.g. "setting.menu.display.theme")
        /// </summary>
        public string MenuPath { get; private set; }

        /// <summary>
        /// last part of key (e.g. "theme" for key "setting.menu.display.theme")
        /// </summary>
        public string Name => MenuPath.Split('.').Last();

        public int Order { get; private set; }

        public bool IsVisible => Order > 0;

        public MenuCustomizationItem(string menuPath, int order)
        {
            MenuPath = menuPath.ToLowerInvariant();
            Order = order;
        }

        public override string ToString() => $"MenuItem (MenuPath:{MenuPath}, Name:{Name}, Order:{Order}, IsVisible:{IsVisible})";
    }
}
