using System;
using System.IO;
using System.Collections;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using System.Collections.ObjectModel;
using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{

    

    class SettingContent_Fonttype : SettingContent_Base
    {

        static public string GetFileName(string value)
        {
            String[] folders = value.Split('/');
            int foldercount = folders.Length;
            if (foldercount > 0) return folders[foldercount - 1];
            return value;
        }


        private static ArrayList FonttypeList = null;



        private static void MakeFonttypeList() 
        {
            FonttypeList = new ArrayList();


            FonttypeList.Add(SystemSettings.FontType);

#if false
            string sharedData = "/opt/usr/data";
            string type = sharedData + "/settings/Alerts";

            Tizen.Log.Debug("NUI", String.Format("fonttype type : {0}", type));

            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(type);

            int i = 0;

            FileInfo[] wavFiles = d.GetFiles("*.wav");
            foreach (FileInfo file in wavFiles)
            {
                Tizen.Log.Debug("NUI", String.Format("[{0}] {1}", i, file.Name));
                FonttypeList.Add(type +"/"+ file.Name);
                i++;
            }

            FileInfo[] mp3Files = d.GetFiles("*.mp3");
            foreach (FileInfo file in mp3Files)
            {
                Tizen.Log.Debug("NUI", String.Format("[{0}] {1}", i, file.Name));
                FonttypeList.Add(type + "/" + file.Name);
                i++;
            }
#endif

        }


        private string[] PickerItems;
        public SettingContent_Fonttype()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            // Init data list
            MakeFonttypeList();

            // Make menu list
            PickerItems = new string[FonttypeList.Count];
            for (int i = 0; i < FonttypeList.Count; i++)
            {
                string type = FonttypeList[i] as string;
                PickerItems[i] = GetFileName(type);
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
            picker.CurrentValue = GetFonttypeIndex();
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

                SetFonttypeIndex(picker.CurrentValue);

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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_FONT_TYPE));
            content.Add(picker);
            content.Add(button);

            return content;
        }


        public static int GetFonttypeIndex()
        {
            string fonttype = GetFonttype();


            for(int i = 0; i < FonttypeList.Count; i++) {
                if (fonttype.Equals(FonttypeList[i])) {
                    return i;
                }

            }

            return -1;
        }

        private static void SetFonttypeIndex(int index)
        {
            string type = FonttypeList[index] as string;
            SetFonttype(type);
        }


        private static void SetFonttype(string fonttype)
        {
            SystemSettings.FontType = fonttype;
        }

        public static string GetFonttype()
        {
            return SystemSettings.FontType;
        }
        public static string GetFonttypeName()
        {
            return GetFonttype();
        }
    }
}

