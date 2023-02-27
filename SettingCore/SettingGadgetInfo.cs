using Tizen.Content.MediaContent;
using Tizen.NUI;

namespace SettingCore
{
    public class SettingGadgetInfo
    {
        public NUIGadgetInfo Pkg { get; private set; }
        public int Order { get; private set; }
        public string ClassName { get; private set; }
        public bool IsMainMenu { get; private set; }
        public string Path { get; private set; }

        public SettingGadgetInfo(NUIGadgetInfo pkg, SettingMenu settingMenu)
        {
            Pkg = pkg;
            Order = settingMenu.DefaultOrder;
            ClassName = settingMenu.ClassName;
            IsMainMenu = settingMenu.IsMainMenu;
            Path = settingMenu.Path;
        }

        public override string ToString() => $"{nameof(SettingGadgetInfo)} (PackageId: {Pkg.PackageId}, ClassName: {ClassName}, Order: {Order}, IsMainMenu: {IsMainMenu}, Path: {Path})";
    }
}
