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
        private Loading loadingIndicator;

        private Window window;

        public ViewManager()
        {
            window = NUIApplication.GetDefaultWindow();
            mainViewModel = new SettingMainViewModel();
        }

        public void SetSplashScreen()
        {
            loadingIndicator = new Loading
            {
                Size2D = new Size2D(window.WindowSize.Width, window.WindowSize.Height),
                CornerRadius = 26.SpToPx(),
                BackgroundColor = Color.White,
                Layout = new LinearLayout
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };

            loadingIndicator.Play();
            window.Add(loadingIndicator);
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

            window.Remove(loadingIndicator);
            window.GetDefaultNavigator().Add(mainView);

            loadingIndicator?.Stop();
            loadingIndicator?.Dispose();
        }
    }
}
