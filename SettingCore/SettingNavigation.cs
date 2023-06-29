using Tizen.NUI;
using Tizen.NUI.Components;

namespace SettingCore
{
    public class SettingNavigation : Navigator
    {
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
