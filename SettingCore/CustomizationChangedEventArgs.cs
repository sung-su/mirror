using System;
using System.Collections.Generic;

namespace SettingCore
{
    public class CustomizationChangedEventArgs : EventArgs
    {
        public CustomizationChangedEventArgs(string menuPath, int order)
        {
            CustomizationItems = new MenuCustomizationItem[] { new MenuCustomizationItem(menuPath, order) };
        }

        public CustomizationChangedEventArgs(IEnumerable<MenuCustomizationItem> items)
        {
            CustomizationItems = items;
        }

        public IEnumerable<MenuCustomizationItem> CustomizationItems { get; private set; }
    }
}
