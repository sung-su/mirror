using System;

using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

using System.Collections.Generic;
using System.Text;

namespace SettingView
{


    class SettingViewBorder : Tizen.NUI.DefaultBorder
    {
        private static readonly string ResourcePath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;

//         private static readonly string MinimalizeIcon = ResourcePath + "/images/minimalize.png";
        private static readonly string MaximalizeIcon = ResourcePath + "/images/maximalize.png";
        private static readonly string RestoreIcon = ResourcePath + "/images/smallwindow.png";
        private static readonly string CloseIcon = ResourcePath + "/images/close.png";
        private static readonly string LeftCornerIcon = ResourcePath + "/images/leftCorner.png";
        private static readonly string RightCornerIcon = ResourcePath + "/images/rightCorner.png";

        private int width = 500;
        private bool hide = false;
        private View borderView;

//        private ImageView minimalizeIcon;
        private ImageView maximalizeIcon;
        private ImageView closeIcon;
        private ImageView leftCornerIcon;
        private ImageView rightCornerIcon;

        public SettingViewBorder() : base()
        {
            //BorderHeight = 50;
            BorderLineThickness = 0;
            ResizePolicy = Window.BorderResizePolicyType.Free;
        }



        public override bool CreateTopBorderView(View topView)
        {
#if false
            if (topView == null)
            {
                return false;
            }
            topView.Layout = new LinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                CellPadding = new Size2D(20, 20),
            };



            var margin = new View
            {
                Size2D = new Size2D(10, 32),
                HeightResizePolicy = ResizePolicyType.FillToParent
            };
            topView.Add(margin);

            PropertyMap titleStyle = new PropertyMap();
            titleStyle.Add("weight", new PropertyValue(600));


            title = new TextLabel()
            {
                Text = "Setting",
                FontStyle = titleStyle,
            };

            topView.Add(title);
            return true;
#else
            return false;
#endif
        }

        public override bool CreateBottomBorderView(View bottomView)
        {
            if (bottomView == null)
            {
                return false;
            }
            bottomView.Layout = new RelativeLayout();
#if false
            minimalizeIcon = new ImageView()
            {
                ResourceUrl = MinimalizeIcon,
                AccessibilityHighlightable = true,
            };
#endif
            maximalizeIcon = new ImageView()
            {
                ResourceUrl = MaximalizeIcon,
                AccessibilityHighlightable = true,
            };

            closeIcon = new ImageView()
            {
                ResourceUrl = CloseIcon,
                AccessibilityHighlightable = true,
            };

            leftCornerIcon = new ImageView()
            {
                ResourceUrl = LeftCornerIcon,
                AccessibilityHighlightable = true,
            };

            rightCornerIcon = new ImageView()
            {
                ResourceUrl = RightCornerIcon,
                AccessibilityHighlightable = true,
            };

#if false
            RelativeLayout.SetRightTarget(minimalizeIcon, maximalizeIcon);
            RelativeLayout.SetRightRelativeOffset(minimalizeIcon, 0.0f);
            RelativeLayout.SetHorizontalAlignment(minimalizeIcon, RelativeLayout.Alignment.End);
#endif
            RelativeLayout.SetRightTarget(maximalizeIcon, closeIcon);
            RelativeLayout.SetRightRelativeOffset(maximalizeIcon, 0.0f);
            RelativeLayout.SetHorizontalAlignment(maximalizeIcon, RelativeLayout.Alignment.End);
            RelativeLayout.SetRightTarget(closeIcon, rightCornerIcon);
            RelativeLayout.SetRightRelativeOffset(closeIcon, 0.0f);
            RelativeLayout.SetHorizontalAlignment(closeIcon, RelativeLayout.Alignment.End);
            RelativeLayout.SetRightRelativeOffset(rightCornerIcon, 1.0f);
            RelativeLayout.SetHorizontalAlignment(rightCornerIcon, RelativeLayout.Alignment.End);
            bottomView.Add(leftCornerIcon);

//            bottomView.Add(minimalizeIcon);

            bottomView.Add(maximalizeIcon);
            bottomView.Add(closeIcon);
            bottomView.Add(rightCornerIcon);


//            minimalizeIcon.TouchEvent += OnMinimizeIconTouched;
            maximalizeIcon.TouchEvent += OnMaximizeIconTouched;
            closeIcon.TouchEvent += OnCloseIconTouched;
            leftCornerIcon.TouchEvent += OnLeftBottomCornerIconTouched;
            rightCornerIcon.TouchEvent += OnRightBottomCornerIconTouched;

#if false
            minimalizeIcon.AccessibilityActivated += (s, e) =>
            {
                MinimizeBorderWindow();
            };
#endif
            maximalizeIcon.AccessibilityActivated += (s, e) =>
            {
                MaximizeBorderWindow();
            };
            closeIcon.AccessibilityActivated += (s, e) =>
            {
                CloseBorderWindow();
            };
#if false
            minimalizeIcon.AccessibilityNameRequested += (s, e) =>
            {
                e.Name = "Minimize";
            };
#endif
            maximalizeIcon.AccessibilityNameRequested += (s, e) =>
            {
                e.Name = "Maximize";
            };
            closeIcon.AccessibilityNameRequested += (s, e) =>
            {
                e.Name = "Close";
            };
            leftCornerIcon.AccessibilityNameRequested += (s, e) =>
            {
                e.Name = "Resize";
            };
            rightCornerIcon.AccessibilityNameRequested += (s, e) =>
            {
                e.Name = "Resize";
            };

//            minimalizeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);

            maximalizeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            closeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            leftCornerIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            rightCornerIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);

            return true;
        }

        public override void CreateBorderView(View borderView)
        {
            this.borderView = borderView;
            borderView.CornerRadius = new Vector4(0.03f, 0.03f, 0.03f, 0.03f);
            borderView.CornerRadiusPolicy = VisualTransformPolicyType.Relative;
            borderView.BackgroundColor = new Color(1, 1, 1, 0.2f);
        }

        public override void OnCreated(View borderView)
        {
            base.OnCreated(borderView);
            UpdateIcons();
        }

        public override bool OnCloseIconTouched(object sender, View.TouchEventArgs e)
        {

            Tizen.Log.Debug("CustomBorder", $"Item Name : {ResourcePath}");

            base.OnCloseIconTouched(sender, e);


            Tizen.Applications.Application.Current.Exit();
            return true;
        }


#if false
        public override bool OnMinimizeIconTouched(object sender, View.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                if (BorderWindow.IsMaximized() == true)
                {
                    BorderWindow.Maximize(false);
                }
                preWinPositonSize = BorderWindow.WindowPositionSize;
                BorderWindow.WindowPositionSize = new Rectangle(preWinPositonSize.X, preWinPositonSize.Y, 500, 0);
            }
            return true;
        }
#endif

        public override void OnRequestResize()
        {
            if (borderView != null)
            {
                borderView.BackgroundColor = new Color(0, 1, 0, 0.3f); // 보더의 배경을 변경할 수 있습니다.
            }
        }

        public override void OnResized(int width, int height)
        {
            if (borderView != null)
            {
                if (this.width > width && hide == false)
                {
                    hide = true;
                }
                else if (this.width < width && hide == true)
                {
                    hide = false;
                }
                borderView.BackgroundColor = new Color(1, 1, 1, 0.3f); //  리사이즈가 끝나면 보더의 색깔은 원래대로 돌려놓습니다.
                base.OnResized(width, height);
                UpdateIcons();
            }
        }

        private void UpdateIcons()
        {
            if (BorderWindow != null && borderView != null)
            {
                if (BorderWindow.IsMaximized() == true)
                {
                    if (maximalizeIcon != null)
                    {
                        maximalizeIcon.ResourceUrl = RestoreIcon;
                    }
                }
                else
                {
                    if (maximalizeIcon != null)
                    {
                        maximalizeIcon.ResourceUrl = MaximalizeIcon;
                    }
                }
            }
        }
    }
}
