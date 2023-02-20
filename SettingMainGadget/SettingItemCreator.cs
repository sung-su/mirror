/*
 *  Copyright (c) 2022 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{

    class SliderItem : View
    {
        public ImageView mIcon = null;
        public Slider mSlider = null;
        public SliderItem()
            : base()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;
        }

    }

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
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Padding = new Extents(8, 0, 5, 5),
            };


            TextLabel label = new TextLabel(text)
            {
                TextColor = Color.DarkGray
            };

            PropertyMap titleStyle = new PropertyMap();
            //            titleStyle.Add("weight", new PropertyValue(600));
            //titleStyle.Add("width", new PropertyValue("expanded"));
            titleStyle.Add("weight", new PropertyValue("bold"));

            label.FontStyle = titleStyle;
            label.FontFamily = "FreeSerif";
            label.PointSize = 18.0f;

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

        public static SliderItem CreateSliderItem(string name, string iconpath, float curvalue)
        {
            //Create Linear Layout
            LinearLayout linearLayout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Extents(20,20, 5, 5),
            };

            var item = new SliderItem
            {
                Name = name,
                Layout = linearLayout
            };
            {
                if (iconpath != null)
                {
                    item.mIcon = new ImageView(iconpath)
                    {
                        Size2D = new Size2D(32, 32)
                    };
                    item.Add(item.mIcon);
                }

                item.mSlider = new Slider()
                {
                    WidthResizePolicy = ResizePolicyType.FillToParent,

                    Margin = new Extents(10, 10, 5,5),

                    TrackThickness = 5,
                    BgTrackColor = new Color(0, 0, 0, 0.1f),
                    SlidedTrackColor = new Color(0.05f, 0.63f, 0.9f, 1),
                    ThumbSize = new Size(20, 20),

                    Direction = Slider.DirectionType.Horizontal,
                    MinValue = 0,
                    MaxValue = 1.0f,
                    CurrentValue = curvalue,

                };
                item.Add(item.mSlider);
            }
            return item;
        }
    }
}
