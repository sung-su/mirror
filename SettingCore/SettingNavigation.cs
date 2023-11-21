using Tizen.NUI;
using Tizen.NUI.Components;

namespace SettingCore
{
    public class SettingNavigation : Navigator
    {
        public SettingNavigation() 
            : base()
        {
            WidthResizePolicy = ResizePolicyType.FillToParent;
            HeightResizePolicy = ResizePolicyType.FillToParent;
        }

        protected override void OnBackNavigation(BackNavigationEventArgs eventArgs)
        {
            if (PageCount > 1)
            {
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().EnableBackNavigation = false;
                GadgetNavigation.NavigateBack();
            }
            else
            {
                NUIApplication.Current?.Exit();
            }
        }
    }
}
