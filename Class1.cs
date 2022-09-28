using System;

public class Class1
{
	public Class1()
	{
        // Create an list item.
        private DefaultLinearItem CreateItem(string text, string subText = null, bool icon = false, bool extra = false)
        {
            var item = new DefaultLinearItem()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Text = text,
                IsSelectable = false, // Item should not be remained as selected state.
            };

            if (subText != null)
            {
                item.SubText = subText;
            }

            CheckBox check = null;
            if (icon)
            {
                check = new CheckBox();
                check.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Tizen.Log.Debug("NUI", "check is selected!\n");
                    }
                    else
                    {
                        Tizen.Log.Debug("NUI", "check is unselected!\n");
                    }
                };
                // Icon is placed at the beginning(left end) of the item.
                item.Icon = check;

                // Do not propagate Pressed/Selected states from item to item.Icon.
                // When item is pressed/clicked/selected, item.Icon is not pressed/clicked/selected.
                item.Icon.PropagatableControlStates = ControlState.Disabled;
            }

            Switch toggle = null;
            if (extra)
            {
                toggle = new Switch();
                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Tizen.Log.Debug("NUI", "toggle is selected!\n");
                    }
                    else
                    {
                        Tizen.Log.Debug("NUI", "toggle is unselected!\n");
                    }
                };
                // Extra is placed at the end(right end) of the item.
                item.Extra = toggle;

                // Do not propagate Pressed/Selected states from item to item.Extra.
                // When item is pressed/clicked/selected, item.Extra is not pressed/clicked/selected.
                item.Extra.PropagatableControlStates = ControlState.Disabled;
            }

            item.Clicked += (o, e) =>
            {
                if (check != null)
                {
                    check.IsSelected = !check.IsSelected;
                }

                if (toggle != null)
                {
                    toggle.IsSelected = !toggle.IsSelected;
                }

                Tizen.Log.Debug("NUI", "item is clicked!\n");
            }
        

            return item;
        }

        // Create a page with scrollable content
        private void CreateSettings()
        {
            // Content of the page which scrolls items vertically.
            var content = new ScrollableBase()
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

            // Create items and add them to the content of the page.
            for (int i = 0; i < 3; i++)
            {
                content.Add(CreateItem("Title"));
                content.Add(CreateItem("Title", "SubTitle"));
                content.Add(CreateItem("Title", null, true));
                content.Add(CreateItem("Title", null, false, true));
                content.Add(CreateItem("Title", null, true, true));
            }

            // Page with AppBar and Content.
            var contentPage = new ContentPage()
            {
                AppBar = new AppBar()
                {
                    Title = "Settings",
                },
                Content = content,
            };

            // Push the page to the default navigator.
            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(contentPage);
        }
    }
}
