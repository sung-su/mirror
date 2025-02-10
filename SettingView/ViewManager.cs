using SettingCore;
using SettingView.ViewModels;
using SettingView.Views;
using Tizen.NUI;
using Tizen.NUI.Binding;
using Tizen.NUI.Components;

namespace SettingView
{
    internal class ViewManager
    {
        private SettingMainView mainView;
        private SettingMainViewModel mainViewModel;

        private Window window;

        public ViewManager()
        {
            mainViewModel = new SettingMainViewModel();
        }

        public void SetupView()
        {
            Logger.Debug("View setup started");

            window = NUIApplication.GetDefaultWindow();
            window.Resized += OnWindowResized;

            mainView = new SettingMainView();
            mainView.BindingContext = mainViewModel;
            mainView.SetBinding(RecyclerView.ItemsSourceProperty, "MainMenuGadgetInfos");

            window.GetDefaultNavigator().Add(mainView);

            Logger.Debug("View setup ended");
        }

        private void OnWindowResized(object sender, Window.ResizedEventArgs e)
        {
            Logger.Debug("OnWindowResized");
            mainView?.UpdateWindowSize();
        }

        public void UpdateViewModel()
        {
            mainViewModel.UpdateViewModel();
            mainView.SetHeader();
        }
    }
}
