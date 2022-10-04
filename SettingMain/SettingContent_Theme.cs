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
    public class ThemeInfo
    {
        private readonly string Name = null;
        private readonly int Value;


        public ThemeInfo(string name, int value)
        {
            Name = name;
            Value = value;
        }


        public string GetName()
        {
            return Name;
        }

        public int GetValue()
        {
            return Value;
        }
    };


    class SettingContent_Theme : SettingContent_Base
    {

        private static readonly ThemeInfo[] ThemeList =
        {
            new ThemeInfo("Light theme", 0),
            new ThemeInfo("Dark theme", 1),
        };

        private string[] PickerItems;
        public SettingContent_Theme()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            PickerItems = new string[ThemeList.Length];
            for (int i = 0; i < ThemeList.Length; i++)
            {
                PickerItems[i] = ThemeList[i].GetName();
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
            picker.CurrentValue = GetThemeIndex();
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

                SetTheme(picker.CurrentValue);

                // Update Widget Content by sending message to pop the fourth page.
                Bundle nextBundle2 = new Bundle();
                nextBundle2.AddItem("WIDGET_ACTION", "POP");
                String encodedBundle2 = nextBundle2.Encode();
                SetContentInfo(encodedBundle2);
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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_THEME));
            content.Add(picker);
            content.Add(button);

            return content;
        }

        void SetTheme(int index)
        {
            SystemSettings.ScreenBacklightTime = ThemeList[index].GetValue();
        }

        public static int GetThemeIndex()
        {

//            Tizen.Log.Debug("NUI", "Theme : " + value.ToString());

            int index = 0;
            return index;
        }
        public static string GetThemeName()
        {
            return ThemeList[GetThemeIndex()].GetName();

        }



#if false
        static char* get_current_theme_str()
        {
            SETTING_TRACE_BEGIN;

            if (!setting_display_theme_change_is_supported())
                return (char*)g_strdup(_("IDS_ST_HEADER_UNAVAILABLE"));

            theme_loader_h tl_handle;
            int result = theme_loader_create(&tl_handle);
            if (result != THEME_MANAGER_ERROR_NONE)
            {
                SETTING_TRACE_ERROR("error %x: theme_loader_create()", result);
                return (char*)g_strdup(_("IDS_ST_HEADER_UNAVAILABLE"));
            }

            char** ids;
            int count;
            result = theme_loader_query_id(tl_handle, &ids, &count);
            if (result == THEME_MANAGER_ERROR_NONE)
            {
                SETTING_TRACE_DEBUG("ids : %d", count);
                for (int i = 0; i < count; i++)
                {
                    SETTING_TRACE_DEBUG("%d : %s", i, ids[i]);

                    theme_h t_handle;
                    result = theme_loader_load(tl_handle, ids[i], &t_handle);
                    if (result == THEME_MANAGER_ERROR_NONE)
                    {
                        char* theme_title;
                        result = theme_get_title(t_handle, &theme_title);
                        if (result == THEME_MANAGER_ERROR_NONE)
                        {
                            SETTING_TRACE_DEBUG("%d's title : %s", i, theme_title);
                            free(theme_title);
                        }
                        theme_destroy(t_handle);
                    }

                    free(ids[i]);
                }
                free(ids);
            }

            theme_h t_handle;
            result = theme_loader_load_current(tl_handle, &t_handle);
            if (result != THEME_MANAGER_ERROR_NONE)
            {
                if (result == THEME_MANAGER_ERROR_NO_SUCH_THEME)
                {
                    SETTING_TRACE_ERROR("error NO_SUCH_THEME: theme_loader_load_current()");
                    theme_loader_destroy(tl_handle);
                    return (char*)g_strdup("No Such Theme");
                }

                SETTING_TRACE_ERROR("error : theme_loader_load_current()");
                theme_loader_destroy(tl_handle);
                return (char*)g_strdup(_("IDS_ST_HEADER_UNAVAILABLE"));
            }

            char* theme_title;
            result = theme_get_title(t_handle, &theme_title);
            if (result != THEME_MANAGER_ERROR_NONE)
            {
                if (result == THEME_MANAGER_ERROR_INVALID_PARAMETER)
                    SETTING_TRACE_ERROR("error INVALID_PARAMETER: theme_get_title()");
                else if (result == THEME_MANAGER_ERROR_OUT_OF_MEMORY)
                    SETTING_TRACE_ERROR("error OUT_OF_MEMORY: theme_get_title()");
                else
                    SETTING_TRACE_ERROR("error : theme_get_title()");

                theme_destroy(t_handle);
                theme_loader_destroy(tl_handle);
                return (char*)g_strdup(_("IDS_ST_HEADER_UNAVAILABLE"));
            }

            theme_destroy(t_handle);
            theme_loader_destroy(tl_handle);

            return theme_title;
        }
#endif




#if false

static void __theme_cb(void *data, Evas_Object *obj, void *event_info)
{
	SETTING_TRACE_BEGIN;

	Elm_Object_Item *item = (Elm_Object_Item *)event_info;
	SettingDisplay *ad = data;

	ret_if(data == NULL);
	retm_if(event_info == NULL, "Invalid argument: event info is NULL");

	elm_genlist_item_selected_set(item, 0);
	Setting_GenGroupItem_Data *list_item = (Setting_GenGroupItem_Data *) elm_object_item_data_get(item);

	SETTING_TRACE("clicking item[%s, %s]", _(list_item->keyStr), list_item->sub_desc);

	theme_loader_h tl_handle;
	int result = theme_loader_create(&tl_handle);
	if(result == THEME_MANAGER_ERROR_NONE) {

		SETTING_TRACE_DEBUG("call theme_loader_set_current(): %s", list_item->sub_desc);
		result = theme_loader_set_current(tl_handle, list_item->sub_desc);
		if(result != THEME_MANAGER_ERROR_NONE) {
			SETTING_TRACE_ERROR("error : theme_loader_set_current()");
		}
		theme_loader_destroy(tl_handle);
	}
	else{
		SETTING_TRACE_ERROR("error : theme_loader_create()");
	}

	if (ad->theme_popup) {
		evas_object_del(ad->theme_popup);
		ad->theme_popup = NULL;
	}

}

#endif

        private static void GetThemeList()
        {

            //ThemeManager 1;
            

#if false
            Evas_Object* menu_glist = NULL;
            Evas_Object* rdg = NULL;



            theme_loader_h tl_handle;
            int result = theme_loader_create(&tl_handle);
            if (result != THEME_MANAGER_ERROR_NONE)
            {
                SETTING_TRACE_ERROR("error %x: theme_loader_create()", result);
                return;
            }



            theme_h t_handle;
            char* curtheme_title = 0;
            result = theme_loader_load_current(tl_handle, &t_handle);
            if (result == THEME_MANAGER_ERROR_NONE)
            {
                char* theme_title;
                result = theme_get_title(t_handle, &theme_title);
                if (result == THEME_MANAGER_ERROR_NONE)
                {
                    curtheme_title = theme_title;
                    SETTING_TRACE_DEBUG("curtheme_title : %s", curtheme_title);
                }

                theme_destroy(t_handle);
            }
            else
            {
                if (result == THEME_MANAGER_ERROR_NO_SUCH_THEME)
                {
                    SETTING_TRACE_ERROR("error NO_SUCH_THEME: theme_loader_load_current()");
                }
                else
                    SETTING_TRACE_ERROR("error : theme_loader_load_current()");
            }

            int cur_radio_num = -1;

            char** ids;
            int count;
            result = theme_loader_query_id(tl_handle, &ids, &count);
            if (result == THEME_MANAGER_ERROR_NONE)
            {
                int radio_num = 0;
                SETTING_TRACE_DEBUG("ids : %d", count);
                for (int i = 0; i < count; i++)
                {
                    SETTING_TRACE_DEBUG("%d : %s", i, ids[i]);

                    result = theme_loader_load(tl_handle, ids[i], &t_handle);
                    if (result == THEME_MANAGER_ERROR_NONE)
                    {
                        char* theme_title;
                        result = theme_get_title(t_handle, &theme_title);
                        if (result == THEME_MANAGER_ERROR_NONE)
                        {
                            SETTING_TRACE_DEBUG("%d's title : %s", i, theme_title);

                            {

                                Setting_GenGroupItem_Data* data_theme = setting_create_Gendial_field_1radio(
                                        menu_glist,
                                        &(ad->itc_1line),
                                        __theme_cb,
                                        ad,
                                        SWALLOW_Type_1RADIO_RIGHT,
                                        rdg,
                                        radio_num,          /* Always ON */
                                        theme_title,
                                        NULL);

                                if (data_theme)
                                {
                                    data_theme->sub_desc = (char*)g_strdup(ids[i]);
                                    data_theme->userdata = ad;
                                    __BACK_POINTER_SET(data_theme);

                                }
                                else
                                {
                                    SETTING_TRACE_ERROR("data_theme is NULL");
                                }

                                if (!isEmptyStr(curtheme_title) && !safeStrCmp(theme_title, curtheme_title))
                                {
                                    cur_radio_num = radio_num;
                                    SETTING_TRACE_DEBUG("cur_radio_num : %d", cur_radio_num);
                                }

                                radio_num++;
                            }

                            free(theme_title);
                        }
                        theme_destroy(t_handle);
                    }

                    free(ids[i]);
                }
                free(ids);
            }

            if (curtheme_title) free(curtheme_title);


            theme_loader_destroy(tl_handle);

            /* update radio */
            if (cur_radio_num >= 0)
                elm_radio_value_set(rdg, cur_radio_num);
#endif
        }


    }
}