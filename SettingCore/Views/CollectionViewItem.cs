using Tizen.NUI.BaseComponents;
using Tizen.NUI;
using Tizen.NUI.Components;
using System;
using System.ComponentModel;

namespace SettingCore.Views
{
    public class CollectionViewItem : RecyclerViewItem
    {
        private readonly ThemeColor BackgroundColors = new ThemeColor(Color.Transparent, Color.Transparent, new Color("#FF6400").WithAlpha(0.16f), new Color("#FFFFFF").WithAlpha(0.16f));
        private readonly ThemeColor TextColors = new ThemeColor(new Color("#090E21"), new Color("#FDFDFD"), new Color("#FF6200"), new Color("#FF8A00"), new Color("#CACACA"), new Color("#666666"));

        private int multiTapCounter;
        private bool touchStarted = false;
        private DateTime multiTapLast = DateTime.MinValue;     

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<EventArgs> MultiTap;
        public event EventHandler<ClickedEventArgs> Clicked;
        public TextLabel TextLabel;
        public TextLabel SubTextLabel;
        public ImageView Icon;

        public CollectionViewItem()
        {
            TouchEvent += OnTouchEvent;
            BackgroundColor = Color.Transparent;
            WidthSpecification = LayoutParamPolicies.MatchParent;
            Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
            };

            Padding = new Extents(0, 0, 8, 8).SpToPx();

            AddIcon();
            AddText();
        }

        private void AddIcon()
        {
            var iconBackground = new View
            {
                CornerRadius = 5.SpToPx(),
                BackgroundColor = Color.Transparent,
                Size = new Size(32, 32).SpToPx(),
                Margin = new Extents(16, 16, 0, 0).SpToPx(),
            };

            Icon = new ImageView
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            iconBackground.Add(Icon);
            Add(iconBackground);
        }

        private void AddText()
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

            TextLabel = new TextLabel()
            {
                PixelSize = 24.SpToPx(),
                TextColor = TextColors.Normal,
            };

            SubTextLabel = new TextLabel()
            {
                PixelSize = 24.SpToPx(),
                TextColor = TextColors.Normal,
            };

            primaryView.Add(TextLabel);
            primaryView.Add(SubTextLabel);
            textContent.Add(primaryView);

            Add(textContent);
        }

        public void OnChangeSelected(bool selected)
        {
            if (selected)
            {
                base.BackgroundColor = BackgroundColors.Selected;

                TextLabel.TextColor = TextColors.Selected;
                SubTextLabel.TextColor = TextColors.Selected;
            }
            else
            {
                base.BackgroundColor = BackgroundColors.Normal;

                TextLabel.TextColor = TextColors.Normal;
                SubTextLabel.TextColor = TextColors.Normal;
            }
        }

        private bool OnTouchEvent(object source, TouchEventArgs e)
        {
            if (MultiTap != null)
            {
                var now = DateTime.Now;
                if (now - multiTapLast > TimeSpan.FromSeconds(2))
                {
                    multiTapCounter = 0;
                    Logger.Verbose("multitap zeroed");
                }

                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    multiTapLast = now;
                    ++multiTapCounter;
                    Logger.Verbose($"multitap {multiTapCounter}");
                }

                if (multiTapCounter == 5)
                {
                    Logger.Verbose("multitap invoke");
                    var handler = MultiTap;
                    handler?.Invoke(this, EventArgs.Empty);
                }
                return false;
            }

            if (Clicked is null)
            {
                return false;
            }

            var state = e.Touch.GetState(0);

            if (state == PointStateType.Down)
            {
                touchStarted = true;
            }
            else if (state == PointStateType.Finished && touchStarted)
            {
                touchStarted = false;

                var handler = Clicked;
                handler?.Invoke(this, new ClickedEventArgs());
            }
            else
            {
                touchStarted = false;
            }

            OnChangeSelected(state == PointStateType.Down);

            return true;
        }
    }
}
