using System.Text;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class TextHeaderListItem : BaseComponent
    {
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#83868F"), new Color("#83868F"), new Color("#666666"), new Color("#666666"));

        private TextLabel textLabel;

        public TextHeaderListItem(string text) : base()
        {
            AccessibilityRole = Role.Label;

            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            textLabel = new TextLabel(text)
            {
                AccessibilityHidden = true,
                Margin = new Extents(24, 0, 16, 16).SpToPx(),
                TextColor = TextColors.Normal,
                PixelSize = 24.SpToPx(),
                Text = text,
            };

            Add(textLabel);

            ThemeManager.ThemeChanged += (s, e) =>
            {
                textLabel.TextColor = TextColors.Normal;
            };
        }

        protected override string AccessibilityGetName() => textLabel.Text;
    }
}
