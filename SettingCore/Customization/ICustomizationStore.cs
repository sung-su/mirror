using System;
using System.Collections.Generic;

namespace SettingCore.Customization
{
    internal interface ICustomizationStore
    {
        public IEnumerable<string> MenuPaths { get; }

        event EventHandler<CustomizationChangedEventArgs> Changed;

        bool SetOrder(string menuPath, int order);
        void UpdateOrder(string menuPath, int order);
        int? GetOrder(string menuPath);

        void Clear();
    }
}
