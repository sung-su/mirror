using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using System.Collections.ObjectModel;
using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{

    

    class SettingContent_Soundmode : SettingContent_Base
    {
        public enum enumSoundmode
        {
            SOUND_MODE_SOUND,
            SOUND_MODE_VIBRATE,
            SOUND_MODE_MUTE
        };

        public class SoundmodeInfo
        {
            private readonly string Name = null;
            private readonly enumSoundmode Value;


            public SoundmodeInfo(string name, enumSoundmode value)
            {
                Name = name;
                Value = value;
            }


            public string GetName()
            {
                return Name;
            }

            public enumSoundmode GetValue()
            {
                return Value;
            }
        };


        private static readonly SoundmodeInfo[] SoundmodeList =
        {
            new SoundmodeInfo(SoundmodeToString(enumSoundmode.SOUND_MODE_SOUND), enumSoundmode.SOUND_MODE_SOUND),
            new SoundmodeInfo(SoundmodeToString(enumSoundmode.SOUND_MODE_MUTE), enumSoundmode.SOUND_MODE_MUTE),
        };



        private string[] PickerItems;
        public SettingContent_Soundmode()
            : base()
        {


            mTitle = Resources.IDS_ST_BUTTON_BACK;

            PickerItems = new string[SoundmodeList.Length];
            for (int i = 0; i < SoundmodeList.Length; i++)
            {
                PickerItems[i] = SoundmodeList[i].GetName();
            }
        }

        protected override View CreateContent(Window window)
        {
            var picker = new Picker()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                // Size = new Size(100, 200),
            };

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(PickerItems);
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = PickerItems.Length - 1;
            picker.CurrentValue = GetSoundmodeIndex();
            Tizen.Log.Debug("NUI", "DisplayedValues : " + picker.DisplayedValues);

            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {

                Tizen.Log.Debug("NUI", String.Format("current : {0}", PickerItems[picker.CurrentValue]));

                SetSoundmodeIndex(picker.CurrentValue);

                RequestWidgetPop();
            };


            var content = new View()
            {

                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };
            content.Add(new TextLabel(Resources.IDS_ST_HEADER_SOUND_MODE));
            content.Add(picker);
            content.Add(button);

            return content;
        }


        public static int GetSoundmodeIndex()
        {
            enumSoundmode mode = GetSoundmode();

            if (mode == enumSoundmode.SOUND_MODE_SOUND) 
                return 0;
            return 1;
        }

        private static void SetSoundmodeIndex(int index)
        {
            SetSoundmode(SoundmodeList[index].GetValue());
        }


        private static void SetSoundmode(enumSoundmode soundmode)
        {
            bool have_sound = false, have_vibrations = false;

            switch (soundmode)
            {
                case enumSoundmode.SOUND_MODE_SOUND:
                    have_sound = true;
                    have_vibrations = false;
                    break;
                case enumSoundmode.SOUND_MODE_VIBRATE:
                    have_sound = false;
                    have_vibrations = true;
                    break;
                case enumSoundmode.SOUND_MODE_MUTE:
                    have_sound = false;
                    have_vibrations = false;
                    break;
            };

            Vconf.SetBool("db/setting/sound/sound_on", have_sound);
            Vconf.SetBool("db/setting/sound/vibration_on", have_vibrations);
        }

        public static enumSoundmode GetSoundmode()
        {
            bool have_sound = Vconf.GetBool("db/setting/sound/sound_on");
                
                
            bool have_vibrations = Vconf.GetBool("db/setting/sound/vibration_on");

            if (have_sound)
                return enumSoundmode.SOUND_MODE_SOUND;
            else if (have_vibrations)
                return enumSoundmode.SOUND_MODE_VIBRATE;
            
            return enumSoundmode.SOUND_MODE_MUTE;
        }
        public static string GetSoundmodeName()
        {
            return SoundmodeToString(GetSoundmode());

        }


        public static string SoundmodeToString(enumSoundmode mode)
        {
	        switch (mode)
	        {
		        case enumSoundmode.SOUND_MODE_SOUND:
			        return Resources.IDS_ST_HEADER_SOUND;
		        case enumSoundmode.SOUND_MODE_VIBRATE:
			        return Resources.IDS_ST_HEADER_VIBRATE;
		        case enumSoundmode.SOUND_MODE_MUTE:
			        return Resources.IDS_ST_HEADER_MUTE;
		        default:
			        return null;
	        }
        }

    }
}

