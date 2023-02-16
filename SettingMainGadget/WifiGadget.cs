using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu
{
    public class WifiGadget : SettingCore.MainMenuGadget
    {
        protected override View OnCreate()
        {
            var text = new TextLabel
            {
                Text = "Wi-Fi",
            };

            return text;
        }

        public override Color ProvideIconColor() => new Color("#FF6200");

        public override string ProvideIconPath() => "main-menu-icons/wifi.svg";

        public override string ProvideTitle() => "Wi-Fi";
    }
}
