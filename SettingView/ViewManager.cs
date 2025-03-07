using SettingCore;
using SettingView.Common;
using SettingView.ViewModels;
using SettingView.Views;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Binding;
using Tizen.NUI.Components;

namespace SettingView
{
    internal class ViewManager
    {
        private SettingMainView mainView;
        private SettingMainViewModel mainViewModel;
        private View splashScreen;

        private Window window;

        public ViewManager()
        {
            window = NUIApplication.GetDefaultWindow();
            mainViewModel = new SettingMainViewModel();
        }

        public void SetSplashScreen()
        {
            splashScreen = new View
            {
                Size2D = new Size2D(window.WindowSize.Width, window.WindowSize.Height),
                CornerRadius = 26.SpToPx(),
                BackgroundColor = Color.White,
                Layout = new RelativeLayout(),
            };

            int iconLen = (int) (119 * AppConstants.ScreenWidthRatio);

            ImageView tizenIcon = new ImageView
            {
                ResourceUrl = Application.Current.DirectoryInfo.Resource + "images/tizen-logo.png",
                CellHorizontalAlignment = HorizontalAlignmentType.Center,
                CellVerticalAlignment = VerticalAlignmentType.Center,
                Size2D = new Size2D(iconLen, iconLen),
            };

            RelativeLayout.SetHorizontalAlignment(tizenIcon, RelativeLayout.Alignment.Center);
            RelativeLayout.SetVerticalAlignment(tizenIcon, RelativeLayout.Alignment.Center);
            splashScreen.Add(tizenIcon);

            window.Add(splashScreen);
        }

        public void SetupMainView()
        {
            Logger.Debug("View setup started");

            window.Resized += OnWindowResized;

            mainView = new SettingMainView();
            mainView.BindingContext = mainViewModel;
            mainView.SetBinding(RecyclerView.ItemsSourceProperty, "MainMenuGadgetInfos");

            Logger.Debug("View setup ended");
        }

        private void OnWindowResized(object sender, Window.ResizedEventArgs e)
        {
            Logger.Debug("OnWindowResized");
            mainView?.UpdateWindowSize();
        }

        public void UpdateMainViewModel()
        {
            mainViewModel.UpdateViewModel();

            window.Remove(splashScreen);
            window.GetDefaultNavigator().Add(mainView);
        }
    }
}
