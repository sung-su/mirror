using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

namespace SettingMain
{
    class SettingWidget_Base : Widget
    {
        private static readonly string resPath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;
        protected const string SETTING_LIST_ICON_PATH_CFG = "/list_icon/";
        protected ListItem[] mListItems;

        public SettingWidget_Base(ListItem[] listItems)
            : base()
        {
            mListItems = listItems;
        }
        protected override void OnCreate(string contentInfo, Window window)
        {
            Bundle bundle = Bundle.Decode(contentInfo);

            mRootView = new View();
            mRootView.BackgroundColor = Color.White;
            mRootView.Size2D = window.Size;
            mRootView.PivotPoint = PivotPoint.Center;
            window.GetDefaultLayer().Add(mRootView);

            CreateVerticalScrollableBase(window);

        }
        private void CreateVerticalScrollableBase(Window window)
        {
            verticalScrollableBase = new ScrollableBase()
            {
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                HideScrollbar = false,
            };
            verticalScrollableBase.ScrollOutOfBound += OnVerticalScrollOutOfBound;

            ListItem[] items = mListItems;
            verticalItems = new Button[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                verticalItems[i] = new Button
                {
                    BackgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f),
                    Position = new Position(0, i * 80),
                    Size = new Size(mRootView.Size2D.Width, 80),
                    PointSize = 12.0f,
                    TextColor = Color.Black,
                    Layout = new LinearLayout()
                    {
                        LinearOrientation = LinearLayout.Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Begin,
                        CellPadding = new Size2D(20, 20),
                    },

                };

                string iconpath = items[i].GetPath();
                if (iconpath.Length > 0)
                {
                    var margin1 = new View
                    {
                        Size2D = new Size2D(10, 32),
                        HeightResizePolicy = ResizePolicyType.FillToParent
                    };
                    verticalItems[i].Add(margin1);

                    ImageView icon = new ImageView(resPath + items[i].GetPath())
                    {
                        Size2D = new Size2D(32, 32),
                        Name = Program.ItemContentNameIcon,
                    };
                    verticalItems[i].Add(icon);
                }

                var margin = new View
                {
                    Size2D = new Size2D(10, 32),
                    HeightResizePolicy = ResizePolicyType.FillToParent
                };
                verticalItems[i].Add(margin);

                PropertyMap titleStyle = new PropertyMap();
                titleStyle.Add("weight", new PropertyValue(600));
                TextLabel title = new TextLabel(items[i].GetLabel())
                {
                    FontStyle = titleStyle,
                    Name = Program.ItemContentNameTitle,
                    Padding = new Extents(8, 8, 8, 8),
                };
                verticalItems[i].Add(title);


                if (i % 2 == 0)
                {
                    verticalItems[i].BackgroundColor = Color.White;
                }
                else
                {
                    verticalItems[i].BackgroundColor = Color.Cyan;
                }
#if false
                var okButton = new Button()
                {
                    Text = "OK",
                };
                okButton.Clicked += (object sender1, ClickedEventArgs e) => { window.GetDefaultNavigator().Pop(); };

                verticalItems[i].Clicked += (object sender, ClickedEventArgs e) =>
                {
                    DialogPage.ShowAlertDialog("Clicked", verticalItems[i].Name, okButton);
                };
#endif

                verticalScrollableBase.Add(verticalItems[i]);
            }
            mRootView.Add(verticalScrollableBase);
        }

        private void OnVerticalScrollOutOfBound(object sender, ScrollOutOfBoundEventArgs e)
        {
#if false
            if (e.Displacement > 80)
            {
                if (e.PanDirection == ScrollOutOfBoundEventArgs.Direction.Down)
                {
                    verticalItems[0].Text = $"Reached at the top, panned displacement is {e.Displacement}.";
                }
            }
            else if (0 - e.Displacement > 80)
            {
                if (e.PanDirection == ScrollOutOfBoundEventArgs.Direction.Up)
                {
                    verticalItems[mListItems.Length - 1].Text = $"Reached at the bottom, panned displacement is {e.Displacement}.";
                }
            }
#endif
        }

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

        protected View mRootView;
        protected ScrollableBase verticalScrollableBase = null;
        protected Button[] verticalItems;
    }
}

