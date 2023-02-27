using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu
{
    public class DisplayGadget : SettingCore.MainMenuGadget
    {
        protected override View OnCreate()
        {
            var text = new TextLabel
            {
                Text = "Theme",
            };
            text.TouchEvent += (sender, e) => {
                if (e.Touch.GetState(0) == PointStateType.Up)
                {
                    NavigateTo("Setting.Menu.Display.Theme");
                }
                return true;
            };

            return text;
        }

        public override Color ProvideIconColor() => new Color("#0075FF");

        public override string ProvideIconPath() => "main-menu-icons/display.svg";

        public override string ProvideTitle() => "Display";
    }
}
