using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class TextHeaderListItem : BaseComponent
    {
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#83868F"), new Color("#83868F"), new Color("#666666"), new Color("#666666"));

        private TextLabel textLabel;

        public TextHeaderListItem(string text, bool multiLine = false) : base()
        {
            AccessibilityRole = Role.Label;

            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            textLabel = new TextLabel(text)
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                AccessibilityHidden = true,
                Margin = new Extents(24, 0, 16, 16).SpToPx(),
                TextColor = TextColors.Normal,
                PixelSize = 24.SpToPx(),
                Text = text,
                MultiLine = multiLine,
            };

            Add(textLabel);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        protected override string AccessibilityGetName() => textLabel.Text;

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            textLabel.TextColor = TextColors.Normal;
        }

        protected override void Dispose(bool disposing)
        {
            ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
            base.Dispose(disposing);
        }
    }
}

