using Tizen.NUI;

namespace SettingCore
{
    public class SettingGadgetInfo
    {
        public NUIGadgetInfo Pkg { get; private set; }
        public int OrderId { get; private set; }
        public string ClassName { get; private set; }
        public bool IsMainMenu { get; private set; }

        public SettingGadgetInfo(NUIGadgetInfo pkg, int orderId, string className, bool isMainMenu)
        {
            Pkg = pkg;
            OrderId = orderId;
            ClassName = className;
            IsMainMenu = isMainMenu;
        }

        public override string ToString() => $"pkgId: {Pkg.PackageId}, class: {ClassName}, orderId: {OrderId}, main: {IsMainMenu}";
    }
}
