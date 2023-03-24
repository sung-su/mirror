using System;
using System.Text;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class TextListItem : BaseComponent
    {
        public static TextListItem CreatePrimaryTextItem(string primaryText) => new TextListItem(primaryText);
        public static TextListItem CreatePrimaryTextItemWithSecondaryText(string primaryText, string secondaryText) => new TextListItem(primaryText, secondaryText: secondaryText);
        public static TextListItem CreatePrimaryTextItemWithSubText(string primaryText, string primarySubText) => new TextListItem(primaryText, primarySubText: primarySubText);

        private readonly ThemeColor BackgroundColors = new ThemeColor(Color.Transparent, Color.Transparent, new Color("#FF6400").WithAlpha(0.16f), new Color("#FFFFFF").WithAlpha(0.16f));
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"));
        private readonly ThemeColor NoActionsTextColors = new ThemeColor(new Color("#83868F"), new Color("#666666"), new Color("#83868F"), new Color("#666666"));

        public string Secondary
        {
            get => secondary.Text;
            set
            {
                if(value != secondary.Text)
                {
                    Remove(secondary);
                    AddSecondaryText(value);
                } 
            }
        }

        private TextLabel primary = new TextLabel();
        private TextLabel primarySubText = new TextLabel();
        private TextLabel secondary = new TextLabel();

        private TextListItem(string primaryText, string primarySubText = "", string secondaryText = "") 
            : base()
        {
            if(!String.IsNullOrEmpty(secondaryText))
            {
                Layout = new FlexLayout()
                {
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                    Direction = FlexLayout.FlexDirection.Row,
                    ItemsAlignment = FlexLayout.AlignmentType.Center
                };
            }else
            {
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }

            primary = new TextLabel(primaryText)
            {
                AccessibilityHidden = true,
                Margin = new Extents(16, 0, 16, 16).SpToPx(),
                TextColor = TextColors.Normal,
                PixelSize = 24.SpToPx(),
                Text = primaryText,
            };

            Add(primary);

            if(!String.IsNullOrEmpty(secondaryText))
            {
                AddSecondaryText(secondaryText);

                Relayout += (s, e) =>
                {
                    secondary.TextColor = isClickedEventEmpty ? NoActionsTextColors.Normal : TextColors.Normal;
                };
            }

            if (!String.IsNullOrEmpty(primarySubText))
            {
                AddPrimarySubText(primarySubText);
            }

            ThemeManager.ThemeChanged += (s, e) => OnChangeSelected(false);
        }

        private void AddSecondaryText(string text)
        {
            secondary = new TextLabel(text)
            {
                AccessibilityHidden = true,
                Margin = new Extents(0, 16, 0, 0).SpToPx(),
                TextColor = TextColors.Normal,
                PixelSize = 20.SpToPx(),
            };

            Add(secondary);
        }

        private void AddPrimarySubText(string text)
        {
            primarySubText = new TextLabel(text)
            {
                AccessibilityHidden = true,
                Margin = new Extents(16, 0, 16, 16).SpToPx(),
                TextColor = TextColors.Normal,
                PixelSize = 24.SpToPx(),
            };

            Add(primarySubText);
        }

        public override void OnChangeSelected(bool selected)
        {
            if (selected)
            {
                base.BackgroundColor = BackgroundColors.Selected;

                primary.TextColor = TextColors.Selected;
                secondary.TextColor = TextColors.Selected;
                primarySubText.TextColor = TextColors.Selected;
            }
            else
            {
                base.BackgroundColor = BackgroundColors.Normal;

                primary.TextColor = TextColors.Normal;
                secondary.TextColor = TextColors.Normal;
                primarySubText.TextColor = TextColors.Normal;
            }
        }

        protected override string AccessibilityGetName()
        {
            StringBuilder sb = new StringBuilder(primary.Text);
            if (!string.IsNullOrEmpty(primarySubText.Text))
            {
                sb.Append($", {primarySubText.Text}");
            }
            if (!string.IsNullOrEmpty(secondary.Text))
            {
                sb.Append($", {secondary.Text}");
            }
            return sb.ToString();
        }
    }
}
