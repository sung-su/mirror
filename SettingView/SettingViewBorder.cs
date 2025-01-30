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

using SettingCore;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingView
{
    public class SettingViewBorder : DefaultBorder
    {
        public static readonly float WindowPadding = 6.0f;
        public static readonly float WindowCornerRadius = 26.0f;

        private static string BorderResourcePath = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "window-border/";
        private static string MinimalizeIconPath = BorderResourcePath + "/window-minimalize.svg";
        private static string MaximalizeIconPath = BorderResourcePath + "/window-maximalize.svg";
        private static string RestoreIconPath = BorderResourcePath + "/window-restore.svg";
        private static string CloseIconPath = BorderResourcePath + "/window-close.svg";
        private static string LeftCornerIconPath = BorderResourcePath + "/window-resize-bottom-left.svg";

        private bool IsLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";
        private Color mBackgroundColor => IsLightTheme ? Color.White.WithAlpha(0.35f) : new Color("#161319").WithAlpha(0.5f);

        private View borderView;
        private ImageView minimalizeIcon;
        private ImageView maximalizeIcon;
        private ImageView closeIcon;
        private ImageView leftCornerIcon;

        public SettingViewBorder() : base()
        {
            ResizePolicy = Window.BorderResizePolicyType.Free;
        }

        public override void CreateBorderView(View borderView)
        {
            this.borderView = borderView;
            borderView.BackgroundColor = mBackgroundColor;

            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (borderView == null)
            {
                return;
            }

            borderView.BackgroundColor = mBackgroundColor;
        }

        public override bool CreateBottomBorderView(View bottomView)
        {
            if (bottomView == null)
            {
                return false;
            }

            BorderLineThickness = (uint)WindowPadding.SpToPx();

            bottomView.HeightSpecification = 48.SpToPx();
            bottomView.Layout = new RelativeLayout()
            {
                Padding = new Extents(0, 24, 0, 0).SpToPx(),
            };

            Size2D iconSize = new Size2D(48, 48).SpToPx();

            minimalizeIcon = new ImageView()
            {
                Size2D = iconSize,
                ResourceUrl = MinimalizeIconPath,
                AccessibilityHighlightable = true,
            };

            maximalizeIcon = new ImageView()
            {
                Size2D = iconSize,
                ResourceUrl = MaximalizeIconPath,
                AccessibilityHighlightable = true,
            };

            closeIcon = new ImageView()
            {
                Size2D = iconSize,
                ResourceUrl = CloseIconPath,
                AccessibilityHighlightable = true,
            };

            leftCornerIcon = new ImageView()
            {
                Size2D = iconSize,
                ResourceUrl = LeftCornerIconPath,
                AccessibilityHighlightable = true,
            };

            minimalizeIcon.TouchEvent += OnMinimizeIconTouched;
            maximalizeIcon.TouchEvent += (s, e) =>
            {
                if (OverlayMode)
                {
                    OverlayMode = false;
                }
                return OnMaximizeIconTouched(s, e);
            };
            closeIcon.TouchEvent += OnCloseIconTouched;
            leftCornerIcon.TouchEvent += OnLeftBottomCornerIconTouched;

            minimalizeIcon.AccessibilityActivated += (s, e) =>
            {
                MinimizeBorderWindow();
            };
            maximalizeIcon.AccessibilityActivated += (s, e) =>
            {
                if (OverlayMode)
                {
                    OverlayMode = false;
                }
                MaximizeBorderWindow();
            };
            closeIcon.AccessibilityActivated += (s, e) =>
            {
                CloseBorderWindow();
            };

            minimalizeIcon.AccessibilityNameRequested += (s, e) =>
            {
                e.Name = "Minimize";
            };
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

            minimalizeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            maximalizeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            closeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            leftCornerIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);

            RelativeLayout.SetRightTarget(minimalizeIcon, maximalizeIcon);
            RelativeLayout.SetRightRelativeOffset(minimalizeIcon, 0.0f);
            RelativeLayout.SetHorizontalAlignment(minimalizeIcon, RelativeLayout.Alignment.End);

            RelativeLayout.SetRightTarget(maximalizeIcon, closeIcon);
            RelativeLayout.SetRightRelativeOffset(maximalizeIcon, 0.0f);
            RelativeLayout.SetHorizontalAlignment(maximalizeIcon, RelativeLayout.Alignment.End);

            RelativeLayout.SetRightRelativeOffset(closeIcon, 1.0f);
            RelativeLayout.SetHorizontalAlignment(closeIcon, RelativeLayout.Alignment.End);

            bottomView.Add(leftCornerIcon);
            bottomView.Add(minimalizeIcon);
            bottomView.Add(maximalizeIcon);
            bottomView.Add(closeIcon);

            return true;
        }

        public override void OnResized(int width, int height)
        {
            if (borderView != null)
            {
                borderView.BackgroundColor = mBackgroundColor;
                Update();
            }
        }

        private void Update()
        {
            if (BorderWindow != null && borderView != null)
            {
                borderView.CornerRadiusPolicy = VisualTransformPolicyType.Absolute;
                borderView.CornerRadius = WindowCornerRadius.SpToPx();

                if (BorderWindow.IsMaximized())
                {
                    maximalizeIcon.ResourceUrl = RestoreIconPath;
                }
                else
                {
                    maximalizeIcon.ResourceUrl = MaximalizeIconPath;
                }
            }
        }
    }
}
