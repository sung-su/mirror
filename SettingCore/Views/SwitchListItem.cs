using System;
using System.Collections.Generic;
using System.Text;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace SettingCore.Views
{
    public class SwitchListItem : BaseComponent
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(Color.Transparent, Color.Transparent, new Color("#FF6400").WithAlpha(0.16f), new Color("#FFFFFF").WithAlpha(0.16f));
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"));

        public Switch Switch { get; private set; }
        private TextLabel primary = new TextLabel();
        private TextLabel primarySubText = new TextLabel();

        public SwitchListItem(string primaryText, string subText = "", bool isSelected = false)
            :base()
        {
            AccessibilityHighlightable = true;
            Padding = new Extents(16, 16, 8, 8).SpToPx();

            Layout = new FlexLayout()
            {
                Justification = FlexLayout.FlexJustification.SpaceBetween,
                Direction = FlexLayout.FlexDirection.Row,
                ItemsAlignment = FlexLayout.AlignmentType.Center
            };

            primary = new TextLabel(primaryText)
            {
                AccessibilityHidden = true,
                TextColor = TextColors.Normal,
                PixelSize = 24.SpToPx(),
                Ellipsis = true,
                LineWrapMode = LineWrapMode.Mixed,
                WidthSpecification = LayoutParamPolicies.MatchParent,
            };

            if(!String.IsNullOrEmpty(subText))
            {
                var view = new View()
                {
                    WidthSpecification = LayoutParamPolicies.MatchParent,
                    Layout = new LinearLayout()
                    {
                        LinearOrientation = LinearLayout.Orientation.Vertical,
                    },
                };

                primarySubText = new TextLabel(subText)
                {
                    AccessibilityHidden = true,
                    TextColor = TextColors.Normal,
                    PixelSize = 24.SpToPx(),
                    Ellipsis = true,
                    LineWrapMode = LineWrapMode.Mixed,
                    WidthSpecification = LayoutParamPolicies.MatchParent,
                };

                view.Add(primary);
                view.Add(primarySubText);

                Add(view);
            }
            else
            {
                FlexLayout.SetFlexShrink(primary, 1f);
                Add(primary);
            }

            Switch = new Switch
            {
                IsSelected = isSelected,
            };
            FlexLayout.SetFlexShrink(Switch, 0f);
            FlexLayout.SetFlexAlignmentSelf(Switch, FlexLayout.AlignmentType.FlexStart);

            Add(Switch);

            Clicked += (s, e) => Switch.IsSelected ^= true;
        }

        public override void OnChangeSelected(bool selected)
        {
            if (selected)
            {
                BackgroundColor = BackgroundColors.Selected;

                primary.TextColor = TextColors.Selected;
                primarySubText.TextColor = TextColors.Selected;
            }
            else
            {
                BackgroundColor = BackgroundColors.Normal;

                primary.TextColor = TextColors.Normal;
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

            return sb.ToString();
        }
    }
}
