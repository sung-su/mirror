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
                    case "getAboutDeviceInfo":
                    {
                        var m = new SystemMemoryUsage();
                            m.Update();
                            var ram = m.Total / (1024 * 1024);
                            Information.TryGetValue("http://tizen.org/feature/screen.width", out int width);
                            Information.TryGetValue("http://tizen.org/feature/screen.height", out int height);
                            return new Dictionary<string, double>
                            {
                                ["Total"] = m.Total,
                                ["Used"] = m.Used,
                                ["Free"] = m.Free,
                                ["Cache"] = m.Cache,
                                ["Swap"] = m.Swap,
                                ["Ram"] = ram,
                                ["width"] = width,
                                ["height"] = height,
                            };
                    }
                    case "getSystemMemoryUsage":
                        {
                            var m = new SystemMemoryUsage();
                            m.Update();
                            var ram = m.Total / (1024 * 1024);
                            return new Dictionary<string, double>
                            {
                                ["Total"] = m.Total,
                                ["Used"] = m.Used,
                                ["Free"] = m.Free,
                                ["Cache"] = m.Cache,
                                ["Swap"] = m.Swap,
                                ["Ram"] = ram,
                            };
                        }
                    case "getResolution":
                        {
                            Information.TryGetValue("http://tizen.org/feature/screen.width", out int width);
                            Information.TryGetValue("http://tizen.org/feature/screen.height", out int height);
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
