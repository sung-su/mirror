using System;

namespace SettingCore
{
    public class SettingMenu
    {
        public string Path { get; private set; }
        public int DefaultOrder { get; private set; }
        public string ClassName { get; private set; }
        public bool IsMainMenu { get; private set; }

        public SettingMenu(string path, int defaultOrder, Type type = null)
        {
            Path = path;
            DefaultOrder = defaultOrder;
            ClassName = type == null ? string.Empty : type.FullName;
            IsMainMenu = path.Split('.').Length == 3;
        }

        public override string ToString() => $"{nameof(SettingMenu)} (Path: {Path}, DefaultOrder: {DefaultOrder}, ClassName: {ClassName}, IsMainMenu: {IsMainMenu})";
    }
}
