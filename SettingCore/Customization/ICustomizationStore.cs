using System;

namespace SettingCore.Customization
{
    internal interface ICustomizationStore
    {
        event EventHandler<CustomizationChangedEventArgs> Changed;

        string CurrentCustomizationLog { get; }

        bool SetOrder(string menuPath, int order);
        void UpdateOrder(string menuPath, int order);
        int GetOrder(string menuPath);

        void Clear();
    }
}
