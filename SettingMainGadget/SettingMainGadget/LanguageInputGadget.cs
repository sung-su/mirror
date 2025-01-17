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
using Tizen.System;
using System.Threading.Tasks;
using Tizen.Applications;

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

        private async Task CreateDisplayLanguageItem()
        {
            Logger.Debug("Start of CreateDisplayLanguageItem");
            await CoreApplication.Post(() =>
            {
                TextListItem displayLanguageItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE)), LanguageInputDisplayLanguageManager.GetDisplayLanguageName(this));
                if (displayLanguageItem != null)
                {
                    displayLanguageItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Language_Display);
                    };
                    sections.Add(MainMenuProvider.Language_Display, displayLanguageItem);
                    content.Add(displayLanguageItem);
                }
                return true;
            });

            Logger.Debug("DisplayLanguageItem Created");
        }

        private async Task CreateKeyboardHeaderItem()
        {
            await CoreApplication.Post(() =>
            {
                TextHeaderListItem keyboardHeaderItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_KEYBOARD)));
                if (keyboardHeaderItem != null)
                {
                    sections.Add(MainMenuProvider.Language_KeyboardHeader, keyboardHeaderItem);
                    content.Add(keyboardHeaderItem);
                }
                return true;
            });

            Logger.Debug("KeyboardHeaderItem Created");
        }

        private async Task CreateKeyboardItem()
        {
            await CoreApplication.Post(() =>
            {
                TextListItem keyboardItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_KEYBOARD)));
                if (keyboardItem != null)
                {
                    keyboardItem.Clicked += (o, e) =>
                    {
                        NavigateTo("Language.InputMethod");
                    };
                    sections.Add("Language.InputMethod", keyboardItem);
                    content.Add(keyboardItem);
                }
                return true;
            });

            Logger.Debug("KeyboardItem Created");
        }

        private async Task CreateInputAssistanceHeaderItem()
        {
            await CoreApplication.Post(() =>
            {
                TextHeaderListItem inputAssistanceHeaderItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_INPUT_ASSISTANCE)));
                if (inputAssistanceHeaderItem != null)
                {
                    sections.Add(MainMenuProvider.Language_InputAssistanceHeader, inputAssistanceHeaderItem);
                    content.Add(inputAssistanceHeaderItem);
                }
                return true;
            });

            Logger.Debug("InputAssistanceHeaderItem Created");
        }

        private async Task CreateAutofillServiceItem()
        {
            await CoreApplication.Post(() =>
            {
                TextListItem autofillServiceItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_AUTOFILL_SERVICE)));
                if (autofillServiceItem != null)
                {
                    autofillServiceItem.Clicked += (o, e) =>
                    {
                        NavigateTo("Language.AutoFill");
                    };
                    sections.Add("Language.AutoFill", autofillServiceItem);
                    content.Add(autofillServiceItem);
                }
                return true;
            });

            Logger.Debug("AutofillServiceItem Created");
        }

        private async Task CreateBodySpeechHeaderItem()
        {
            await CoreApplication.Post(() =>
            {
                TextHeaderListItem bodySpeechHeaderItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SPEECH)));
                if (bodySpeechHeaderItem != null)
                {
                    sections.Add(MainMenuProvider.Language_BodySpeech, bodySpeechHeaderItem);
                    content.Add(bodySpeechHeaderItem);
                }
                return true;
            });

            Logger.Debug("BodySpeechHeaderItem Created");
        }

        private async Task CreateVoiceControl()
        {
            await CoreApplication.Post(() =>
            {
                TextListItem voiceControl = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_VOICE_BODY_VOICE_CONTROL_ABB2)));
                if (voiceControl != null)
                {
                    voiceControl.Clicked += (o, e) =>
                    {
                        NavigateTo("Language.VoiceControl");
                    };
                    sections.Add("Language.VoiceControl", voiceControl);
                    content.Add(voiceControl);
                }
                return true;
            });

            Logger.Debug("Voice Control Created");
        }

        private async Task CreateLanguageTTS()
        {
            await CoreApplication.Post(() =>
            {
                TextListItem languageTTS = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_VOICE_HEADER_TEXT_TO_SPEECH_HTTS)));
                if (languageTTS != null)
                {
                    languageTTS.Clicked += (o, e) =>
                    {
                        NavigateTo("Language.TTS");
                    };
                    sections.Add("Language.TTS", languageTTS);
                    content.Add(languageTTS);
                }
                return true;
            });

            Logger.Debug("LanguageTTS Created");
        }

        private async Task CreateLanguageSTT()
        {
            await CoreApplication.Post(() =>
            {
                TextListItem languageSTT = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_VOICE_HEADER_SPEECH_TO_TEXT_HSTT)));
                if (languageSTT != null)
                {
                    languageSTT.Clicked += (o, e) =>
                    {
                        NavigateTo("Language.STT");
                    };
                    sections.Add("Language.STT", languageSTT);
                    content.Add(languageSTT);
                }
                return true;
            });

            Logger.Debug("LanguageSTT Created");
        }

        private async void CreateView()
        {
            Logger.Debug("CreateView Starts in LanguageInputGadget");
            sections.RemoveAllSectionsFromView(content);

            await CreateDisplayLanguageItem();
            await CreateKeyboardHeaderItem();
            await CreateKeyboardItem();
            await CreateInputAssistanceHeaderItem();
            await CreateAutofillServiceItem();
            await CreateBodySpeechHeaderItem();
            await CreateVoiceControl();
            await CreateLanguageTTS();
            await CreateLanguageSTT();

            Logger.Debug("View Created");
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
