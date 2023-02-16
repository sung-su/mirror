using Tizen.NUI.BaseComponents;

namespace Setting.Menu.Display
{
    public class ThemeGadget : SettingCore.MenuGadget
    {
        protected override View OnCreate()
        {
            return new TextLabel
            {
                Text = "Light Theme / Dark Theme",
            };
        }

        public override string ProvideTitle() => "Theme";
    }
}
