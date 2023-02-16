using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu
{
    public class SoundGadget : SettingCore.MainMenuGadget
    {
        protected override View OnCreate()
        {
            var text = new TextLabel
            {
                Text = "Sound",
            };

            return text;
        }

        public override Color ProvideIconColor() => new Color("#DB3069");

        public override string ProvideIconPath() => "main-menu-icons/sound.svg";

        public override string ProvideTitle() => "Sound";
    }
}
