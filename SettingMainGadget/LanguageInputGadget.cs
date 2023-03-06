using SettingAppTextResopurces.TextResources;
using SettingMainGadget.LanguageInput;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu
{
    public class LanguageInputGadget : SettingCore.MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color("#205493");

        public override string ProvideIconPath() => GetResourcePath("language_input.svg");

        public override string ProvideTitle() => Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT;

        private DefaultLinearItem displayLanguageItem;

        protected override View OnCreate()
        {
            SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;

            return CreateView();
        }

        protected override void OnDestroy()
        {
            SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;

            base.OnDestroy();
        }

        private View CreateView()
        {
            var content = new ScrollableBase
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

            displayLanguageItem = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE, LanguageInputDisplayLanguageManager.GetDisplayLanguageName());
            if (displayLanguageItem != null)
            {
                displayLanguageItem.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.LanguageInput.DisplayLanguage");
                };
                content.Add(displayLanguageItem);
            }

            content.Add(SettingMain.SettingItemCreator.CreateItemStatic(""));
            content.Add(SettingMain.SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_KEYBOARD));

            var item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_KEYBOARD);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("inputmethod@org.tizen.cssetting-inputmethod");
                };
                content.Add(item);
            }

            content.Add(SettingMain.SettingItemCreator.CreateItemStatic(""));
            content.Add(SettingMain.SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_INPUT_ASSISTANCE));

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_AUTOFILL_SERVICE);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("autofill@org.tizen.cssetting-autofill");
                };
                content.Add(item);
            }

            content.Add(SettingMain.SettingItemCreator.CreateItemStatic(""));
            content.Add(SettingMain.SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_SPEECH));

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_BODY_VOICE_CONTROL_ABB2);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("voicecontrol@org.tizen.cssetting-voicecontrol");
                };
                content.Add(item);
            }

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_HEADER_TEXT_TO_SPEECH_HTTS);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("tts@org.tizen.cssetting-tts");
                };
                content.Add(item);
            }

            item = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_VOICE_HEADER_SPEECH_TO_TEXT_HSTT);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("stt@org.tizen.cssetting-stt");
                };
                content.Add(item);
            }

            return content;
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, LocaleLanguageChangedEventArgs e)
        {
            // TODO : reload all text or use the TranslatableText
            if (displayLanguageItem != null)
                displayLanguageItem.SubText = LanguageInputDisplayLanguageManager.GetDisplayLanguageName();
        }
    }
}
