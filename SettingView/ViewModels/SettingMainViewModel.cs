using SettingCore;
using SettingView.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SettingView.ViewModels
{
    internal class SettingMainViewModel
    {
        private List<MainMenuInfo> catchedMenuItems;
        private IEnumerable<SettingGadgetInfo> visibleMenuItems;
        public ObservableCollection<GadgetInfoModel> MainMenuGadgetInfos { get; set; }

        public SettingMainViewModel()
        {
            MainMenuGadgetInfos = new ObservableCollection<GadgetInfoModel>();
            LoadGadgetItems();
        }

        private void LoadGadgetItems(bool customizationChanged = false)
        {
            Logger.Debug("Gadget items load started");

            catchedMenuItems = MainMenuInfo.CacheMenu;

            if (catchedMenuItems.Count == 0 || customizationChanged)
            {
                var mainMenus = GadgetManager.Instance.GetMainWithCurrentOrder();
                if (!mainMenus.Any())
                    return;

                visibleMenuItems = mainMenus.Where(i => i.IsVisible);
                if (!visibleMenuItems.Any())
                    return;
            }
            else
            {
                Logger.Debug("Loaded Gadgets from cache");
            }

            Logger.Debug("Gadget items load ended");
        }

        public void UpdateViewModel()
        {
            if (catchedMenuItems != null && catchedMenuItems.Count != 0)
            {
                foreach (var menu in catchedMenuItems)
                {
                    MainMenuGadgetInfos.Add(new GadgetInfoModel(menu.IconPath, menu.IconColorHex, menu.Title, menu.Path));
                }
            }
            else if (visibleMenuItems != null)
            {
                Task.Run(() =>
                {
                    catchedMenuItems = new List<MainMenuInfo>();
                    foreach (var gadgetInfo in visibleMenuItems)
                    {
                        if (MainMenuInfo.Create(gadgetInfo) is MainMenuInfo menu)
                        {
                            MainMenuGadgetInfos.Add(new GadgetInfoModel(menu.IconPath, menu.IconColorHex, menu.Title, menu.Path));
                            catchedMenuItems.Add(menu);
                        }
                    }

                    Logger.Debug("Catching items -> Count : " + catchedMenuItems.Count);
                    MainMenuInfo.UpdateCache(catchedMenuItems);
                });
            }
        }
    }
}
