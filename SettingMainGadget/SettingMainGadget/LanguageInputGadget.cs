using SettingMainGadget.TextResources;
using SettingCore;
using SettingCore.Views;
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
        public override Color ProvideIconColor() => new Color(IsLightTheme ? "#205493" : "#2560A8");

        public override string ProvideIconPath() => GetResourcePath("language_input.svg");

        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT));

        private View content;
        private Sections sections = new Sections();

        protected override View OnCreate()
        {
            base.OnCreate();

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

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(DisplayGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }

        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);
            TextListItem displayLanguageItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE)), LanguageInputDisplayLanguageManager.GetDisplayLanguageName(this));
            if (displayLanguageItem != null)
            {
                displayLanguageItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_Display);
                };
                sections.Add(MainMenuProvider.Language_Display, displayLanguageItem);
            }

            TextHeaderListItem keyboardHeaderItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_KEYBOARD)));
            if (keyboardHeaderItem != null)
            {
                sections.Add(MainMenuProvider.Language_KeyboardHeader, keyboardHeaderItem);
            }

            TextListItem keyboardItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_KEYBOARD)));
            if (keyboardItem != null)
            {
                keyboardItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_InputMethod);
                };
                sections.Add(MainMenuProvider.Language_InputMethod, keyboardItem);
            }

            TextHeaderListItem inputAssistanceHeaderItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_INPUT_ASSISTANCE)));
            if (inputAssistanceHeaderItem != null)
            {
                sections.Add(MainMenuProvider.Language_InputAssistanceHeader, inputAssistanceHeaderItem);
            }

            TextListItem autofillServiceItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_AUTOFILL_SERVICE)));
            if (autofillServiceItem != null)
            {
                autofillServiceItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_AutoFill);
                };
                sections.Add(MainMenuProvider.Language_AutoFill, autofillServiceItem);
            }

            TextHeaderListItem bodySpeachHeaderItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SPEECH)));
            if (bodySpeachHeaderItem != null)
            {
                sections.Add(MainMenuProvider.Language_BodySpeach, bodySpeachHeaderItem);
            }

            TextListItem voiceControl = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_VOICE_BODY_VOICE_CONTROL_ABB2)));
            if (voiceControl != null)
            {
                voiceControl.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_VoiceControl);
                };
                sections.Add(MainMenuProvider.Language_VoiceControl, voiceControl);
            }

            TextListItem languageTTS = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_VOICE_HEADER_TEXT_TO_SPEECH_HTTS)));
            if (languageTTS != null)
            {
                languageTTS.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_TTS);
                };
                sections.Add(MainMenuProvider.Language_TTS, languageTTS);
            }

            TextListItem languageSTT = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_VOICE_HEADER_SPEECH_TO_TEXT_HSTT)));
            if (languageSTT != null)
            {
                languageSTT.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Language_STT);
                };
                sections.Add(MainMenuProvider.Language_STT, languageSTT);
            }

            var customization = GetCustomization().OrderBy(c => c.Order);
            Logger.Debug($"customization: {customization.Count()}");
            foreach (var cust in customization)
            {
                string visibility = cust.IsVisible ? "visible" : "hidden";
                Logger.Verbose($"Customization: {cust.MenuPath} - {visibility} - {cust.Order}");
                if (cust.IsVisible && sections.TryGetValue(cust.MenuPath, out View row))
                {
                    content.Add(row);
                }
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
