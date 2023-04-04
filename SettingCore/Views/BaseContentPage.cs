using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class BaseContentPage : ContentPage
    {
        protected override string AccessibilityGetName() => AppBar?.Title;
    }
}
