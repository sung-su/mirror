#include <iostream>
#include <Elementary.h>
#include <Ecore.h>
#include <launchpad.h>
#include <aul.h>
#include <glib.h>
#include <glib-unix.h>
#include <unistd.h>
#include "flutter.h"
#include "tizen_log.h"


class App : public FlutterApp {
public:
    bool OnCreate() {
        if (FlutterApp::OnCreate()) {
            // TODO: Need to solve RegisterPlugins issue
            //RegisterPlugins(this);
        }
        return IsRunning();
    }
};

// To precreate window(EFL/DALI), argc and argv should be passed.
// But, create callback of loader doesnot pass that to parameter.
// So, store argc and argv and use that to precreation.
// If window precreation code moves to managed, removed below code.
//static int __argc;
//static char **__argv;

static const char* KEY_APP_TYPE = "--appType";
Ecore_Fd_Handler* fd_handler_ = nullptr;
loader_receiver_cb receiver_ = nullptr;

static Eina_Bool ProcessFdHandler(void* user_data, Ecore_Fd_Handler* handler) {
    int fd = ecore_main_fd_handler_fd_get(handler);
    if (fd == -1) {
        TizenLog::Error("Failed to get fd from fd handler");
        exit(-1);
    }

    if (ecore_main_fd_handler_active_get(handler, ECORE_FD_READ)) {
        TizenLog::Error("Process FD handler");
        receiver_(fd);
    } else if (ecore_main_fd_handler_active_get(handler, ECORE_FD_ERROR)) {
        TizenLog::Error("ECORE_FD_ERROR");
        close(fd);
        exit(-1);
    }
    return ECORE_CALLBACK_CANCEL;
}

static void __loader_create_cb(bundle *extra, int type, void *user_data)
{
    TizenLog::Error("__loader_create_cb");
    char *appType = NULL;
    if (bundle_get_str(extra, KEY_APP_TYPE, &appType) != BUNDLE_ERROR_NONE) {
        appType = NULL;
    }
    TizenLog::Error("AppType : %s", appType);
}

static int __loader_launch_cb(int argc, char **argv, const char *app_path,
        const char *appid, const char *pkgid, const char *pkg_type,
        void *user_data) {

    TizenLog::Error("__loader_launch_cb");
    const char* root_path = aul_get_app_root_path();
    chdir(root_path);
    TizenLog::Error("App root path : %s", root_path);
    TizenLog::Error("app_path : %s", app_path);
    TizenLog::Error("App id : %s", appid);
    TizenLog::Error("pkg id : %s", pkgid);
    TizenLog::Error("pkg type : %s", pkg_type);
    return 0;
}

static int __loader_terminate_cb(int argc, char **argv, void *user_data)
{
    TizenLog::Error("__loader_terminate_cb");
    App app;
    return app.Run(argc, argv);
}

static void __adapter_loop_begin(void *user_data)
{
    TizenLog::Error("loop begin");
    ecore_main_loop_begin();
}

static void __adapter_loop_quit(void *user_data)
{
    TizenLog::Error("loop quit");
    ecore_main_loop_quit();
}

static void __adapter_add_fd(void *user_data, int fd, loader_receiver_cb receiver)
{
    TizenLog::Error("__adapter_add_fd , fd : %d", fd);
    fd_handler_ = ecore_main_fd_handler_add(fd,
        static_cast<Ecore_Fd_Handler_Flags>(ECORE_FD_READ | ECORE_FD_ERROR),
        ProcessFdHandler, nullptr, nullptr, nullptr);
    if (fd_handler_ == nullptr) {
        TizenLog::Error("fd handler is nullptr");
        close(fd);
        exit(-1);
    }
  receiver_ = receiver;
}

static void __adapter_remove_fd(void *user_data, int fd)
{
    TizenLog::Error("__adapter_remove_fd , fd : %d", fd);
    if (fd_handler_) {
        ecore_main_fd_handler_del(fd_handler_);
        fd_handler_ = nullptr;
        receiver_ = nullptr;
    }
}

extern "C" int loaderMain(int argc, char *argv[])
{
    std::cout << "@@@ loaderMain()" << std::endl;
    TizenLog::Error("Start Flutter loader.");
    elm_init(argc, argv);
    ecore_init();

    loader_lifecycle_callback_s callbacks = {
        .create = __loader_create_cb,
        .launch = __loader_launch_cb,
        .terminate = __loader_terminate_cb
    };

    loader_adapter_s adapter = {
        .loop_begin = __adapter_loop_begin,
        .loop_quit = __adapter_loop_quit,
        .add_fd = __adapter_add_fd,
        .remove_fd = __adapter_remove_fd
    };

    int ret = launchpad_loader_main(argc, argv, &callbacks, &adapter, NULL);
    TizenLog::Error("launchpad_loader_main ---- end!!! ret : %d", ret);
    return ret;
}


extern "C" int launcherMain(int argc, char *argv[])
{
    std::cout << "@@@ launcherMain()" << std::endl;
    TizenLog::Error("Start Flutter launcher.");
    elm_init(argc, argv);
    ecore_init();

    loader_lifecycle_callback_s callbacks = {
        .create = __loader_create_cb,
        .launch = __loader_launch_cb,
        .terminate = __loader_terminate_cb
    };

    loader_adapter_s adapter = {
        .loop_begin = __adapter_loop_begin,
        .loop_quit = __adapter_loop_quit,
        .add_fd = __adapter_add_fd,
        .remove_fd = __adapter_remove_fd
    };

    int ret = launchpad_loader_main(argc, argv, &callbacks, &adapter, NULL);
    TizenLog::Error("launchpad_launcher_main ---- end!!! ret : %d", ret);
    return ret;
}

int version = 3;

int main(int argc, char *argv[]) {
    std::cout << "hello world" << std::endl;
    //std::vector<char*> vargs;
    //what kind of info we need to pass to flutter?
    std::cout << "@@@ version=[" << version << "]"<< std::endl;
    std::cout << "@@@ argc=[" << argc << "]"<< std::endl;
    for (int i = 1; i < argc; i++) {
        std::cout << "@@@ argv[" << i << "]=[" << argv[i] << "]"<< std::endl;
        return launcherMain(argc, argv);
    }
    return loaderMain(argc, argv);
}
