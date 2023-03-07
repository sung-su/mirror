using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.NUI;

namespace SettingCore
{
    public class RoundedDialogPage : DialogPage
    {
        private const float WindowOuterCornerRadius = 26f;
        private const float WindowBorderThickness = 6f;

        public RoundedDialogPage() : base()
        {
            // create custom scrim, which matches parent's width and height - correct size after window resize
            Scrim = CreateCustomScrim();

            // change CorenerRadius at Scrim, which is protected property
            Scrim.CornerRadius = (WindowOuterCornerRadius - WindowBorderThickness).SpToPx();
        }

        // Modified copy from https://github.com/Samsung/TizenFX/blob/dd96e3c04563a8df1de0c9f7201a0fb9f9ca7c36/src/Tizen.NUI.Components/Controls/Navigation/DialogPage.cs#L314-L330
        private View CreateCustomScrim()
        {
            var scrimStyle = ThemeManager.GetStyle("Tizen.NUI.Components.DialogPage.Scrim");
            var scrim = new VisualView(scrimStyle);

            //FIXME: Needs to set proper size to Scrim.
            //scrim.Size = NUIApplication.GetDefaultWindow().Size;
            scrim.WidthSpecification = LayoutParamPolicies.MatchParent;
            scrim.HeightSpecification = LayoutParamPolicies.MatchParent;

            scrim.TouchEvent += (object source, TouchEventArgs e) =>
            {
                if ((EnableDismissOnScrim == true) && (e.Touch.GetState(0) == PointStateType.Up))
                {
                    this.Navigator.Pop();
                }
                return true;
            };

            return scrim;
        }

        public static new void ShowAlertDialog(string title, string message, params View[] actions)
        {
            var dialogPage = new RoundedDialogPage()
            {
                Content = new AlertDialog()
                {
                    Title = title,
                    Message = message,
                    Actions = actions,
                },
            };

            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(dialogPage);
        }
    }
}
