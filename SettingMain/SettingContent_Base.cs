using System;
using System.ComponentModel;
//using System.Diagnostics;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

namespace SettingMain
{
    abstract class SettingContent_Base : Widget
    {


        protected static readonly string resPath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;
        protected const string SETTING_ICON_PATH_CFG = "/icons/";
        protected const string SETTING_LIST_ICON_PATH_CFG = "/icons/list_icon/";

        // Create an Static Text
        protected View CreateItemStatic(string text)
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

            TextLabel label = new TextLabel(text);
            label.TextColor = Color.Black;

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

        protected View CreateItemTitle(string text)
        {
            TextLabel label = new TextLabel(text);
            label.TextColor = Color.Black;

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
        protected DefaultLinearItem CreateItemWithCheck(string text, string subText = null, bool icon = false, bool extra = false)
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
        protected DefaultLinearItem CreateItemWithIcon(string text, string iconpath, string subText = null, bool extra = false)
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
        protected DefaultLinearItem CreateItemWithSlider(string text, string iconpath)
        {
            var item = new DefaultLinearItem()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Text = text,
                IsSelectable = false, // Item should not be remained as selected state.
            };

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

            return item;
        }

        protected View CreateSliderItem(string name, string iconpath, int levelcout)
        {
            //Create Linear Layout
            LinearLayout linearLayout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                CellPadding = new Size2D(10, 10)
            };

            var item = new View();
            item.Name = name;
            item.Layout = linearLayout;
            {
                var leftpadding = new View()
                {
                    Size2D = new Size2D(10, 5),
                };
                item.Add(leftpadding);

                if (iconpath != null)
                {
                    var icon = new ImageView(iconpath);
                    icon.Size2D = new Size2D(32, 32);
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
                    MaxValue = levelcout-1,
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

        private void OnValueChanged(object sender, SliderValueChangedEventArgs args)
        {
        }

        private void OnSlidingStarted(object sender, SliderSlidingStartedEventArgs args)
        {
        }

        private void OnSlidingFinished(object sender, SliderSlidingFinishedEventArgs args)
        {
        }




        /// ///////////////////////////////////////////////////////////////////////////
        /// 

        protected void RequestWidgetPush(string widgetid)
        {
            if (mWindow == null) return;

            // Update Widget Content by sending message to add the third page in advance.
            Bundle nextBundle = new Bundle();
            nextBundle.AddItem("WIDGET_ID", widgetid);
            nextBundle.AddItem("WIDGET_WIDTH", mWindow.Size.Width.ToString());
            nextBundle.AddItem("WIDGET_HEIGHT", mWindow.Size.Height.ToString());
            nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
            nextBundle.AddItem("WIDGET_ACTION", "PUSH");
            String encodedBundle = nextBundle.Encode();
            SetContentInfo(encodedBundle);
        }

        protected void RequestWidgetPop()
        {
            // Update Widget Content by sending message to pop the fourth page.
            Bundle nextBundle2 = new Bundle();
            nextBundle2.AddItem("WIDGET_ACTION", "POP");
            String encodedBundle2 = nextBundle2.Encode();
            SetContentInfo(encodedBundle2);
        }

        ///////////////////////////////////////////////////////////
        ///

        protected Window mWindow;

        protected string mTitle;
        protected ContentPage mPage;

        public SettingContent_Base()
            : base()
        {
            mWindow = null;
            mPage = null;
        }

        protected override void OnCreate(string contentInfo, Window window)
        {
            Bundle bundle = Bundle.Decode(contentInfo);

            window.BackgroundColor = Color.Transparent;

            mWindow = window;

            mPage = new ContentPage()
            {
                Content = CreateContent(window),
                AppBar = CreateAppBar(mTitle),
            };

            window.Add(mPage);

        }

        protected virtual AppBar CreateAppBar(string title)
        {
            var appBar = new AppBar()
            {
                Title = title,
                AutoNavigationContent = false,
            };

            var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar");

            var navigationContent = new Button(((AppBarStyle)appBarStyle).BackButton);
            navigationContent.Clicked += (o, e) =>
            {
                // Update Widget Content by sending message to pop the third page.
                Bundle nextBundle = new Bundle();
                nextBundle.AddItem("WIDGET_ACTION", "POP");
                String encodedBundle = nextBundle.Encode();
                SetContentInfo(encodedBundle);
            };
            appBar.NavigationContent = navigationContent;
            //appBarStyle.Dispose();

            return appBar;
        }

        protected abstract View CreateContent(Window window);

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            Tizen.Log.Error("widget", "OnResume \n");
            base.OnResume();
        }

        protected override void OnResize(Window window)
        {
            base.OnResize(window);
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {

            base.OnTerminate(contentInfo, type);
        }

        protected override void OnUpdate(string contentInfo, int force)
        {
            base.OnUpdate(contentInfo, force);
        }
    }
}

