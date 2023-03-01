namespace SettingCore.Customization
{
    internal interface ICustomizationStore
    {
        string CurrentCustomizationLog { get; }

        bool SetOrder(string menuPath, int order);
        void UpdateOrder(string menuPath, int order);
        int GetOrder(string menuPath);

        void Clear();
    }
}
