using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMainGadget;
using SettingMainGadget.LanguageInput;
using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.PhonenumberUtils;
using Tizen.System;

namespace Setting.Menu
{
    public class LanguageInputGadget : SettingCore.MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color("#205493");

        public override string ProvideIconPath() => GetResourcePath("language_input.svg");

        public override string ProvideTitle() => Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT;

        private DefaultLinearItem displayLanguageItem;
        private View content;
        private List<View> contentItems = new List<View>();

        protected override View OnCreate()
        {
            SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;

            content = new ScrollableBase
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            CreateView();

            return content;
        }

        protected override void OnDestroy()
        {
            SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;

            base.OnDestroy();
        }

        private void CreateView()
        {
            foreach (var items in contentItems)
            {
                content.Remove(items);
            }
            contentItems.Clear();

            displayLanguageItem = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE, LanguageInputDisplayLanguageManager.GetDisplayLanguageName());
            if (displayLanguageItem != null)
            {
                displayLanguageItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_Display);
                };
                contentItems.Add(displayLanguageItem);
            }

            contentItems.Add(SettingMain.SettingItemCreator.CreateItemStatic(""));
            contentItems.Add(SettingMain.SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_KEYBOARD));

            var item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_KEYBOARD);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_InputMethod);
                };
                contentItems.Add(item);
            }

            contentItems.Add(SettingMain.SettingItemCreator.CreateItemStatic(""));
            contentItems.Add(SettingMain.SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_INPUT_ASSISTANCE));

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_AUTOFILL_SERVICE);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_AutoFill);
                };
                contentItems.Add(item);
            }

            contentItems.Add(SettingMain.SettingItemCreator.CreateItemStatic(""));
            contentItems.Add(SettingMain.SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_SPEECH));

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_BODY_VOICE_CONTROL_ABB2);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_VoiceControl);
                };
                contentItems.Add(item);
            }

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_HEADER_TEXT_TO_SPEECH_HTTS);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_TTS);
                };
                contentItems.Add(item);
            }

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_HEADER_SPEECH_TO_TEXT_HSTT);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_STT);
                };
                contentItems.Add(item);
            }

            foreach (var view in contentItems)
            {
                content.Add(view);
            }
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, LocaleLanguageChangedEventArgs e)
        {
            if (content != null)
            {
                CreateView();
            }
        }
    }
}
