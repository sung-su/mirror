using System;

namespace SettingCore.Customization
{
    public class CustomizationChangedEventArgs : EventArgs
    {
        public CustomizationChangedEventArgs(string menuPath, int order)
        {
            MenuPath = menuPath;
            Order = order;
        }

        public string MenuPath { get; private set; }
        public int Order { get; private set; }
        public bool IsVisible => Order > 0;
    }
}
