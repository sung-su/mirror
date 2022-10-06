using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingView
{
    class SettingItemCreator
    {
        // Create an Static Text
        public static View CreateItemStatic(string text)
        {
            var item = new View()
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    CellPadding = new Size2D(10, 10)
                },
            };

            var leftpadding = new View()
            {
                Size2D = new Size2D(8, 5),
            };
            item.Add(leftpadding);

            TextLabel label = new TextLabel(text)
            {
                TextColor = new Color(0.1f, 0.1f, 0.1f, 0.0f)
            };

            PropertyMap titleStyle = new PropertyMap();
            //            titleStyle.Add("weight", new PropertyValue(600));
            //titleStyle.Add("width", new PropertyValue("expanded"));
            titleStyle.Add("weight", new PropertyValue("bold"));

            label.FontStyle = titleStyle;
            label.FontFamily = "FreeSerif";
            label.PointSize = 14.0f;

            item.Add(label);

            return item;
        }

        public static View CreateItemTitle(string text)
        {
            TextLabel label = new TextLabel(text)
            {
                TextColor = Color.Black
            };

            PropertyMap titleStyle = new PropertyMap();
            //            titleStyle.Add("weight", new PropertyValue(600));
            //titleStyle.Add("width", new PropertyValue("expanded"));
            titleStyle.Add("weight", new PropertyValue("bold"));

            label.FontStyle = titleStyle;
            label.FontFamily = "FreeSerif";
            label.PointSize = 18.0f;

            return label;
        }


        // Create an list item with checkbox.
        public static DefaultLinearItem CreateItemWithCheck(string text, string subText = null, bool icon = false, bool extra = false)
        {
            var item = new DefaultLinearItem()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Text = text,
                IsSelectable = false, // Item should not be remained as selected state.
            };

            if (subText != null)
            {
                item.SubText = subText;
            }

            CheckBox check = null;
            if (icon)
            {
                check = new CheckBox();
                check.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Tizen.Log.Debug("NUI", "check is selected!\n");
                    }
                    else
                    {
                        Tizen.Log.Debug("NUI", "check is unselected!\n");
                    }
                };
                // Icon is placed at the beginning(left end) of the item.
                item.Icon = check;

                // Do not propagate Pressed/Selected states from item to item.Icon.
                // When item is pressed/clicked/selected, item.Icon is not pressed/clicked/selected.
                item.Icon.PropagatableControlStates = ControlState.Disabled;
            }

            Switch toggle = null;
            if (extra)
            {
                toggle = new Switch();
                item.Extra = toggle;

                // Do not propagate Pressed/Selected states from item to item.Extra.
                // When item is pressed/clicked/selected, item.Extra is not pressed/clicked/selected.
                item.Extra.PropagatableControlStates = ControlState.Disabled;
            }

            item.Clicked += (o, e) =>
            {
                if (check != null)
                {
                    check.IsSelected = !check.IsSelected;
                }

                if (toggle != null)
                {
                    toggle.IsSelected = !toggle.IsSelected;
                }

                Tizen.Log.Debug("NUI", "item is clicked!\n");
            };



            return item;
        }

        // Create an list item  with icon
        public static DefaultLinearItem CreateItemWithIcon(string text, string iconpath, string subText = null, bool extra = false)
        {
            var item = new DefaultLinearItem()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Text = text,
                IsSelectable = false, // Item should not be remained as selected state.
            };

            if (subText != null)
            {
                item.SubText = subText;
            }

            ImageView icon = null;

            if (iconpath.Length > 0)
            {
                icon = new ImageView(iconpath)
                {
                    Size2D = new Size2D(32, 32),
                    //Name = Program.ItemContentNameIcon,
                };
                // Icon is placed at the beginning(left end) of the item.
                item.Icon = icon;
            }

            Switch toggle = null;
            if (extra)
            {
                toggle = new Switch();
                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Tizen.Log.Debug("NUI", "toggle is selected!\n");
                    }
                    else
                    {
                        Tizen.Log.Debug("NUI", "toggle is unselected!\n");
                    }
                };
                // Extra is placed at the end(right end) of the item.
                item.Extra = toggle;

                // Do not propagate Pressed/Selected states from item to item.Extra.
                // When item is pressed/clicked/selected, item.Extra is not pressed/clicked/selected.
                item.Extra.PropagatableControlStates = ControlState.Disabled;
            }

            item.Clicked += (o, e) =>
            {
                if (toggle != null)
                {
                    toggle.IsSelected = !toggle.IsSelected;
                }

                Tizen.Log.Debug("NUI", "item is clicked!\n");
            };



            return item;
        }

        // Create an list item  with icon
        public static DefaultLinearItem CreateItemWithSlider(string text, string iconpath)
        {
            var item = new DefaultLinearItem()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Text = text,
                IsSelectable = false, // Item should not be remained as selected state.
            };

            if (iconpath.Length > 0)
            {
                ImageView icon = new ImageView(iconpath)
                {
                    Size2D = new Size2D(32, 32),
                    //Name = Program.ItemContentNameIcon,
                };
                // Icon is placed at the beginning(left end) of the item.
                item.Icon = icon;
            }

            return item;
        }

        public static View CreateSliderItem(string name, string iconpath, int levelcout)
        {
            //Create Linear Layout
            LinearLayout linearLayout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                CellPadding = new Size2D(10, 10)
            };

            var item = new View
            {
                Name = name,
                Layout = linearLayout
            };
            {
                var leftpadding = new View()
                {
                    Size2D = new Size2D(10, 5),
                };
                item.Add(leftpadding);

                if (iconpath != null)
                {
                    var icon = new ImageView(iconpath)
                    {
                        Size2D = new Size2D(32, 32)
                    };
                    item.Add(icon);
                }

                var slider = new Slider()
                {
                    //WidthResizePolicy = ResizePolicyType.FillToParent,
                    //Size2D = new Size2D(100, 32),
                    //Name = Program.ItemContentNameDescription,

                    TrackThickness = 5,
                    BgTrackColor = new Color(0, 0, 0, 0.1f),
                    SlidedTrackColor = new Color(0.05f, 0.63f, 0.9f, 1),
                    ThumbSize = new Size(20, 20),

                    Direction = Slider.DirectionType.Horizontal,
                    MinValue = 0,
                    MaxValue = levelcout - 1,
                    CurrentValue = 10,

                };
                slider.ValueChanged += OnValueChanged;
                slider.SlidingStarted += OnSlidingStarted;
                slider.SlidingFinished += OnSlidingFinished;
                item.Add(slider);

                var rightpadding = new View()
                {
                    Size2D = new Size2D(10, 5),
                };
                item.Add(rightpadding);


            }
            return item;
        }

        private static void OnValueChanged(object sender, SliderValueChangedEventArgs args)
        {
        }

        private static void OnSlidingStarted(object sender, SliderSlidingStartedEventArgs args)
        {
        }

        private static void OnSlidingFinished(object sender, SliderSlidingFinishedEventArgs args)
        {
        }


    }
}
