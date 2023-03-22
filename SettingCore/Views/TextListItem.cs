using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class Colors
    {
        public ThemeColor Background = new ThemeColor();
        public ThemeColor Text = new ThemeColor(); // gray if no actions
    }

    public class TextListItem : BaseComponent
    {
        public static TextListItem CreatePrimaryTextItem(string primaryText) => new TextListItem(primaryText);
        public static TextListItem CreatePrimaryTextItemWithSecondaryText(string primaryText, string secondaryText) => new TextListItem(primaryText, secondaryText: secondaryText);
        public static TextListItem CreatePrimaryTextItemWithSubText(string primaryText, string primarySubText) => new TextListItem(primaryText, primarySubText: primarySubText);

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
        private Colors Colors = new Colors();

        public TextListItem(string primaryText, string primarySubText = "", string secondaryText = "") 
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

            SetColors();

            primary = new TextLabel(primaryText)
            {
                AccessibilityHidden = true,
                Margin = new Extents(16, 0, 16, 16).SpToPx(),
                TextColor = Colors.Text.Normal,
                PixelSize = 24.SpToPx(),
                Text = primaryText,
            };

            Add(primary);

            if(!String.IsNullOrEmpty(secondaryText))
            {
                AddSecondaryText(secondaryText);
            }

            if (!String.IsNullOrEmpty(primarySubText))
            {
                AddPrimarySubText(primarySubText);
            }
        }

        private void AddSecondaryText(string text)
        {
            secondary = new TextLabel(text)
            {
                AccessibilityHidden = true,
                Margin = new Extents(0, 16, 0, 0).SpToPx(),
                TextColor = Colors.Text.Normal,
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
                TextColor = Colors.Text.Normal,
                PixelSize = 24.SpToPx(),
            };

            Add(primarySubText);
        }

        private void SetColors()
        {
            Colors.Background.SetSelectedColor(new Color("#FF6400").WithAlpha(0.16f), new Color("#FFFFFF").WithAlpha(0.16f));

            Colors.Text.SetNormalColor(new Color("#090E21"), new Color("#CACACA"));
            Colors.Text.SetSelectedColor(new Color("#FF6200"), new Color("#FF8A00"));
        }

        public override void OnChangeSelected(bool selected)
        {
            if (selected)
            {
                BackgroundColor = Colors.Background.Selected;

                primary.TextColor = Colors.Text.Selected;
                secondary.TextColor = Colors.Text.Selected;
                primarySubText.TextColor = Colors.Text.Selected;
            }
            else
            {
                BackgroundColor = Colors.Background.Normal;

                primary.TextColor = Colors.Text.Normal;
                secondary.TextColor = Colors.Text.Normal;
                primarySubText.TextColor = Colors.Text.Normal;
            }
        }

        protected override string AccessibilityGetName()
        {
            if (!String.IsNullOrEmpty(secondary.Text) || !String.IsNullOrEmpty(primarySubText.Text))
            {
                return $"{primary.Text}, {secondary.Text}{primarySubText.Text}";
            }

            return $"{primary.Text}";
        }
    }
}
