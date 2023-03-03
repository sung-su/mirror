using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu
{
    public class BluetoothGadget : SettingCore.MainMenuGadget
    {
        protected override View OnCreate()
        {
            var text = new TextLabel
            {
                Text = "Bluetooth",
            };

            return text;
        }

        public override Color ProvideIconColor() => new Color("#FF6200");

        public override string ProvideIconPath() => GetResourcePath("bluetooth.svg");

        public override string ProvideTitle() => "Bluetooth";
    }
}
