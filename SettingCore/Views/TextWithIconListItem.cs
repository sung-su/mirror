using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore.Views
{
    public class TextWithIconListItem : BaseComponent
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(Color.Transparent, Color.Transparent, new Color("#FF6400").WithAlpha(0.16f), new Color("#FFFFFF").WithAlpha(0.16f));
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"), new Color("#CACACA"), new Color("#666666"));
        private readonly ThemeColor NoActionsTextColors = new ThemeColor(new Color("#83868F"), new Color("#666666"), Color.Transparent, Color.Transparent);
        private readonly ThemeColor SubTextColors = new ThemeColor(new Color("#83868F"), new Color("#83868F"), new Color("#83868F"), new Color("#83868F"), new Color("#CACACA"), new Color("#666666"));
        private readonly ThemeColor IconColors = new ThemeColor(Color.White, Color.Black, Color.White, Color.Black);

        public string SubText
        {
            get => primarySubText.Text;
            set
            {
                if (value != primarySubText.Text)
                {
                    primarySubText.Text = value;
                }
            }
        }

        private ImageView icon;
        private ImageVisual iconVisual;

        private TextLabel primary;
        private TextLabel primarySubText;
        private TextLabel secondary;
        private TextLabel secondarySubTextLabel;

        public TextWithIconListItem(string primaryText, Color iconColor, string iconPath = "", string subText = "", string secondaryText = "", string secondarySubText = "")
            : base()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;
            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
            };

            Padding = new Extents(0, 0, 8, 8).SpToPx();

            AddIcon(iconColor, iconPath);

            AddText(primaryText, subText, secondaryText, secondarySubText);
        }

        private void AddIcon(Color color, string iconPath)
        {
            var iconBackground = new View
            {
                CornerRadius = 5.SpToPx(),
                BackgroundColor = color,
                Size = new Size(32, 32).SpToPx(),
                Margin = new Extents(16, 16, 0, 0).SpToPx(),
            };

            if (!String.IsNullOrEmpty(iconPath))
            {
                iconVisual = new ImageVisual
                {
                    URL = iconPath,
                    FittingMode = FittingModeType.ScaleToFill,
                };

                icon = new ImageView
                {
                    Image = iconVisual.OutputVisualMap,
                    WidthSpecification = LayoutParamPolicies.MatchParent,
                    HeightSpecification = LayoutParamPolicies.MatchParent,
                };
                iconBackground.Add(icon);
            }

            Add(iconBackground);
        }

        private void AddText(string primaryText, string subText, string secondaryText, string secondarySubText)
        {
            var textContent = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Layout = new FlexLayout()
                {
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                    Direction = FlexLayout.FlexDirection.Row,
                    ItemsAlignment = FlexLayout.AlignmentType.Center
                },
            };

            var primaryView = new View()
            {
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };

            // primary text
            primary = new TextLabel(primaryText)
            {
                Margin = new Extents(0, 0, 0, 0).SpToPx(),
                TextColor = TextColors.Normal,
                PixelSize = 24.SpToPx(),
            };

            primaryView.Add(primary);
            textContent.Add(primaryView);

            // primary sub text
            if (!String.IsNullOrEmpty(subText))
            {
                primarySubText = new TextLabel(subText)
                {
                    TextColor = TextColors.Normal,
                    PixelSize = 24.SpToPx(),
                };
                primaryView.Add(primarySubText);
            }

            // secondary
            if (!String.IsNullOrEmpty(secondaryText))
            {
                var secondaryView = new View()
                {
                    Layout = new LinearLayout()
                    {
                        LinearOrientation = LinearLayout.Orientation.Vertical,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.End,
                    },
                };

                secondary = new TextLabel(secondaryText)
                {
                    TextColor = SubTextColors.Normal,
                    PixelSize = 24.SpToPx(),
                };
                secondaryView.Add(secondary);

                // secondary subtext
                if (!String.IsNullOrEmpty(secondarySubText))
                {
                    secondarySubTextLabel = new TextLabel(secondarySubText)
                    {
                        TextColor = SubTextColors.Normal,
                        PixelSize = 24.SpToPx(),
                    };
                    secondaryView.Add(secondarySubTextLabel);
                }
                textContent.Add(secondaryView);
            }

            Add(textContent);
        }
    }
}
