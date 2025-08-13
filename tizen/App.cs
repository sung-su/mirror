using System;
using System.Collections.Generic;
using Tizen.Applications;
using Tizen.Flutter.Embedding;
using Tizen.System;

namespace Runner
{
    public class App : FlutterApplication
    {
        protected override void OnCreate()
        {
            UserPixelRatio = 2.0;
            base.OnCreate();

            GeneratedPluginRegistrant.RegisterPlugins(this);

            var channel = new MethodChannel("tizenfx");
            channel.SetMethodCallHandler(async (call) =>
            {
                switch (call.Method)
                {
                    case "getSystemMemoryUsage":
                        {
                            var m = new SystemMemoryUsage();
                            m.Update();
                            return new Dictionary<string, double>
                            {
                                ["Total"] = m.Total,
                                ["Used"] = m.Used,
                                ["Free"] = m.Free,
                                ["Cache"] = m.Cache,
                                ["Swap"] = m.Swap,
                                ["Ram"] = ((m.Total / 1024) / 1024),
                            };
                        }
                    case "getResolution":
                        {
                            int width = -1;
                            int height = -1;
                            Information.TryGetValue("http://tizen.org/feature/screen.width", out width);
                            Information.TryGetValue("http://tizen.org/feature/screen.height", out height);

                            return new Dictionary<string, int>
                            {
                                ["width"] = width,
                                ["height"] = height,
                            };
                        }
                    default:
                        throw new System.NotImplementedException(call.Method);
                }
            });
        }

        static void Main(string[] args)
        {
            var app = new App();
            app.Run(args);
        }
    }
}
