using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.Applications;

namespace SettingView
{
    public class Program : NUIApplication
    {
        public Program(string styleSheet, Size2D windowSize, Position2D windowPosition, IBorderInterface borderInterface)
            : base(styleSheet, windowSize, windowPosition, borderInterface)
        {

        }
        protected override void OnCreate()
        {
            base.OnCreate();
            Window window = Window.Instance;
            window.BackgroundColor = Color.Blue;
            window.KeyEvent += OnKeyEvent;

            Bundle bundle = new Bundle();
            bundle.AddItem("COUNT", "1");
            String encodedBundle = bundle.Encode();

            Tizen.Log.Error("SettingWidget", "REQUEST \n");

            //mWidgetView = WidgetViewManager.Instance.AddWidget("main@org.tizen.SettingMain", encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
            mWidgetView = WidgetViewManager.Instance.AddWidget("aboutdevice@org.tizen.SettingMain", encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
            mWidgetView.Position = new Position(0, 0);
            window.GetDefaultLayer().Add(mWidgetView);
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        static void Main(string[] args)
        {
            var appCustomBorder = new SettingViewBorder();
            var app = new Program("", new Size2D(800, 400), new Position2D(300, 100), appCustomBorder);

            app.Run(args);
        }

        WidgetView mWidgetView;
    }
}
