#include "runner.h"

#include "generated_plugin_registrant.h"

class App : public FlutterApp {
 public:
  bool OnCreate() {
    user_pixel_ratio_ = 2.0;
    if (FlutterApp::OnCreate()) {
      RegisterPlugins(this);
    }
    return IsRunning();
  }
};

int main(int argc, char *argv[]) {
  App app;
  return app.Run(argc, argv);
}
