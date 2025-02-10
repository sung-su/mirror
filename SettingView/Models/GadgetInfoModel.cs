using SettingCore;
using System.Windows.Input;
using Tizen.NUI.Binding;

namespace SettingView.Models
{
    internal class GadgetInfoModel
    {
        public GadgetInfoModel(string iconPath, string iconClolor, string title, string path)
        {
            IconPath = iconPath;
            IconColorHex = iconClolor;
            Title = title;
            Path = path;
            GadgetSelectCommand = new Command(OnGadgetClicked);
        }

        public string IconPath { get; set; }
        public string IconColorHex { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public ICommand GadgetSelectCommand { get; set; }

        private void OnGadgetClicked()
        {
            Logger.Debug("Gadget Item clicked: Title -> " + Title);
            GadgetNavigation.NavigateTo(Path);
        }
    }
}
