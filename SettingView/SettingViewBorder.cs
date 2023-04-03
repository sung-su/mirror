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
    class SettingViewBorder : Tizen.NUI.DefaultBorder
    {
        /// <summary>
        /// Padding between outer (semi-transparent) window border background and inner (opaque white) window content background in pixels. This PX value is used to calculate SP.
        /// </summary>
        public static readonly float WindowPadding = 6.0f;

        public static readonly float WindowCornerRadius = 26.0f;

        private bool IsLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";
        private Color mBackgroundColor => IsLightTheme ? Color.White.WithAlpha(0.35f) : new Color("#161319").WithAlpha(0.5f);

        private static string GetResourcePath(string relativePath) => System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, relativePath);
        private static string MinimalizeIconPath = GetResourcePath("window-border/window-minimalize.svg");
        private static string MaximalizeIconPath = GetResourcePath("window-border/window-maximalize.svg");
        private static string RestoreIconPath = GetResourcePath("window-border/window-restore.svg");
        private static string CloseIconPath = GetResourcePath("window-border/window-close.svg");
        private static string LeftCornerIconPath = GetResourcePath("window-border/window-resize-bottom-left.svg");

        private View borderView;

        private ImageView minimalizeIcon;
        private ImageView maximalizeIcon;
        private ImageView closeIcon;
        private ImageView leftCornerIcon;

        public SettingViewBorder() : base()
        {
            ResizePolicy = Window.BorderResizePolicyType.Free;
            MinSize = new Size2D(500, 300);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
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

            minimalizeIcon = new ImageView()
            {
                ResourceUrl = MinimalizeIconPath,
                AccessibilityHighlightable = true,
            };

            maximalizeIcon = new ImageView()
            {
                ResourceUrl = MaximalizeIconPath,
                AccessibilityHighlightable = true,
            };

            closeIcon = new ImageView()
            {
                ResourceUrl = CloseIconPath,
                AccessibilityHighlightable = true,
            };

            leftCornerIcon = new ImageView()
            {
                ResourceUrl = LeftCornerIconPath,
                AccessibilityHighlightable = true,
            };

            minimalizeIcon.TouchEvent += OnMinimizeIconTouched;
            maximalizeIcon.TouchEvent += OnMaximizeIconTouched;
            closeIcon.TouchEvent += OnCloseIconTouched;
            leftCornerIcon.TouchEvent += OnLeftBottomCornerIconTouched;

            minimalizeIcon.AccessibilityActivated += (s, e) =>
            {
                MinimizeBorderWindow();
            };
            maximalizeIcon.AccessibilityActivated += (s, e) =>
            {
                MaximizeBorderWindow();
            };
            closeIcon.AccessibilityActivated += (s, e) =>
            {
                CloseBorderWindow();
            };

            minimalizeIcon.AccessibilityNameRequested += (s, e) =>
            {
                // TODO: get from Resource file
                e.Name = "Minimize";
            };
            maximalizeIcon.AccessibilityNameRequested += (s, e) =>
            {
                // TODO: get from Resource file
                e.Name = "Maximize";
            };
            closeIcon.AccessibilityNameRequested += (s, e) =>
            {
                // TODO: get from Resource file
                e.Name = "Close";
            };
            leftCornerIcon.AccessibilityNameRequested += (s, e) =>
            {
                // TODO: get from Resource file
                e.Name = "Resize";
            };

            minimalizeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            maximalizeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            closeIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);
            leftCornerIcon.SetAccessibilityReadingInfoTypes(Tizen.NUI.BaseComponents.AccessibilityReadingInfoTypes.Name);

            // Apply scalable changes. Has to be called when NUIApplication is initilialized, due to SpToPx() usage.
            BorderLineThickness = (uint)WindowPadding.SpToPx();

            var size = new Size(48, 48).SpToPx();
            leftCornerIcon.Size = size;
            minimalizeIcon.Size = size;
            maximalizeIcon.Size = size;
            closeIcon.Size = size;

            // Default BorderHeight is 50 and does not adopt to content height, so HAVE TO change to SVGs height.
            bottomView.SizeHeight = 48.SpToPx();

            var controls = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                },
                Margin = new Extents(0, (ushort)(WindowCornerRadius - WindowPadding), 0, 0).SpToPx(),
            };
            controls.Add(minimalizeIcon);
            controls.Add(maximalizeIcon);
            controls.Add(closeIcon);

            bottomView.Layout = new FlexLayout
            {
                Direction = FlexLayout.FlexDirection.Row,
                Justification = FlexLayout.FlexJustification.SpaceBetween,
            };
            bottomView.Add(leftCornerIcon);
            bottomView.Add(controls);

            return true;
        }

        public override void CreateBorderView(View borderView)
        {
            if (borderView == null)
            {
                return;
            }

            this.borderView = borderView;
            borderView.BackgroundColor = mBackgroundColor;
        }

        public override void OnCreated(View borderView)
        {
            base.OnCreated(borderView);
            Update();
        }

        public override bool OnCloseIconTouched(object sender, View.TouchEventArgs e)
        {
            base.OnCloseIconTouched(sender, e);

            Tizen.Applications.Application.Current.Exit();
            return true;
        }

        public override void OnRequestResize()
        {
            if (borderView == null)
            {
                return;
            }

            borderView.BackgroundColor = Color.Green.WithAlpha(0.3f);
        }

        public override void OnResized(int width, int height)
        {
            if (borderView == null)
            {
                return;
            }

            borderView.BackgroundColor = mBackgroundColor;
            Update();
        }

        private void Update()
        {
            if (BorderWindow == null || borderView == null || maximalizeIcon == null)
            {
                return;
            }

            borderView.CornerRadiusPolicy = VisualTransformPolicyType.Absolute;

            if (BorderWindow.IsMaximized())
            {
                maximalizeIcon.ResourceUrl = RestoreIconPath;
                borderView.CornerRadius = 0;
            }
            else
            {
                maximalizeIcon.ResourceUrl = MaximalizeIconPath;
                borderView.CornerRadius = WindowCornerRadius.SpToPx();
            }
        }
    }
}
