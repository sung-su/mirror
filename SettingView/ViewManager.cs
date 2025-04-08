using SettingCore;
using SettingView.Common;
using SettingView.ViewModels;
using SettingView.Views;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Binding;
using Tizen.NUI.Components;
using System.Collections;

namespace SettingView
{
    internal class ViewManager
    {
        private SettingMainView mainView;
        private SettingMainViewModel mainViewModel;
        private View splashScreen;
        private bool viewSetupCompleted;
        private bool pendingUpdate;

        private Window window;

        static class RecyclerViewBindings
        {
            public static BindingProperty<RecyclerView, IEnumerable> ItemsSourceProperty { get; } = new BindingProperty<RecyclerView, IEnumerable>
            {
                Setter = (v, value) => v.ItemsSource = value,
            };
        }

        public ViewManager()
        {
            window = NUIApplication.GetDefaultWindow();

            viewSetupCompleted = false;
            pendingUpdate = false;
        }

        public void InitViewModel()
        {
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
            Logger.Debug("SetupMainView started");

            window.Resized += OnWindowResized;

            var session = new BindingSession<SettingMainViewModel>();
            mainView = new SettingMainView();
            mainView.BindingContextChanged += (sender, e) =>
            {
                if (mainView.BindingContext is SettingMainViewModel model)
                {
                    session.ViewModel = model;
                }
            };

            mainView.SetBinding(session, RecyclerViewBindings.ItemsSourceProperty, "MainMenuGadgetInfos");
            mainView.BindingContext = mainViewModel;

            viewSetupCompleted  = true;
            if (pendingUpdate)
            {
                pendingUpdate = false;
                UpdateMainViewModel();
            }

            Logger.Debug("SetupMainView ended");
        }

        private void OnWindowResized(object sender, Window.ResizedEventArgs e)
        {
            Logger.Debug("OnWindowResized");
            mainView?.UpdateWindowSize();
        }

        public void UpdateMainViewModel()
        {
            Logger.Debug("UpdateMainViewModel started");

            if (!viewSetupCompleted)
            {
                pendingUpdate = true;
                return;
            }

            mainViewModel.UpdateViewModel();

            window.Remove(splashScreen);
            window.GetDefaultNavigator().Add(mainView);
        }
    }
}
