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
        }

        static void Main(string[] args)
        {
            var app = new App();
            app.Run(args);
        }
    }
}
