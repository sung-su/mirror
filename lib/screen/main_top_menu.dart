import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/notifications/notification_panel.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/top_menu_avatar_item.dart';
import 'package:tizen_fs/widgets/top_menu_button_item.dart';
import 'package:tizen_fs/widgets/top_menu_icon_item.dart';
import 'package:tizen_fs/profiles/switch_profile_panel.dart';

class MainTopMenu extends StatefulWidget {
  const MainTopMenu({super.key, required this.pageController});

  final PageController pageController;

  @override
  State<MainTopMenu> createState() => _MainTopMenuState();
}

class _MainTopMenuState extends State<MainTopMenu> {
  final List<String> pages = ['Tizen', 'Home'];
  final int _itemCount = 4;

  int _selected = 1;

  @override
  void initState() {
    super.initState();
  }

  void _movePage(int pageIndex) {
    widget.pageController.animateToPage(
      pageIndex,
      duration: $style.times.fast,
      curve: Curves.easeInOut,
    );
  }

  void setSelected(int index) {
    if (_selected != index) {
      setState(() {
        _selected = index;
      });

      if (_selected > 0 && _selected < (_itemCount - 2)) {
        _movePage(_selected - 1);
      } else if (_selected == 0) {
        _showAccountPanel();
      }
    }
  }

  void _showNotificationPanel() {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Close",
      barrierColor: Colors.transparent,
      transitionDuration: $style.times.pageTransition,
      pageBuilder: (
        BuildContext buildContext,
        Animation animation,
        Animation secondaryAnimation,
      ) {
        return NotificationsPanel();
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return SlideTransition(
          position: Tween<Offset>(
            begin: const Offset(0.5, 0),
            end: Offset.zero,
          ).animate(animation),
          child: FadeTransition(opacity: animation, child: child),
        );
      },
    ).then((_) {
      setState(() {
        _selected = _itemCount - 2;
      });
    });
  }

  void _showAccountPanel() {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Close",
      barrierColor: Colors.transparent,
      transitionDuration: const Duration(milliseconds: 80),
      pageBuilder: (
        BuildContext buildContext,
        Animation animation,
        Animation secondaryAnimation,
      ) {
        return AccountPanel();
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return FadeTransition(opacity: animation, child: child);
      },
    );
  }

  void _onFocusChanged(bool hasFocus) {
    if (hasFocus) {
      if (_selected == 0) {
        setState(() {
          _selected = 1;
        });
      }
    }
    Provider.of<BackdropProvider>(context, listen: false).isZoomIn = hasFocus;
  }

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        setSelected((_selected > 0) ? (_selected - 1) : _selected);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        final selected =
            (_selected < _itemCount - 1) ? (_selected + 1) : _selected;
        if (selected == _itemCount - 1) {
          _showNotificationPanel();
        } else {
          setSelected(selected);
        }
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter) {
        if (_selected == 0) {
          AppRouter.router.push(ScreenPaths.poc);
        }
        if (_selected == 2) {
          AppRouter.router.push(ScreenPaths.settings);
        }
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.goBack ||
          event.logicalKey == LogicalKeyboardKey.escape ||
          event.physicalKey == PhysicalKeyboardKey.escape) {
        AppRouter.router.push(ScreenPaths.main);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      autofocus: true,
      onFocusChange: _onFocusChanged,
      onKeyEvent: _onKeyEvent,
      child: Builder(
        builder: (context) {
          return Padding(
            padding: const EdgeInsets.fromLTRB(43, 20, 48, 0),
            child: Row(
              children: [
                TopMenuAvatarItem(
                  imageUrl: null,
                  text: pages[0],
                  isSelected: 0 == _selected,
                  onPressed: () {
                    Focus.of(context).requestFocus();
                    setSelected(0);
                  },
                ),
                SizedBox(width: 15),
                TopMenuButtonItem(
                  text: pages[1],
                  isSelected: 1 == _selected,
                  isFocused: Focus.of(context).hasFocus,
                  onPressed: () {
                    Focus.of(context).requestFocus();
                    setSelected(1);
                  },
                ),
                // TopMenuButtonItem(
                //   text: pages[2],
                //   isSelected: 2 == _selected,
                //   isFocused: Focus.of(context).hasFocus,
                //   onPressed: () {
                //     Focus.of(context).requestFocus();
                //     setSelected(2);
                //   },
                // ),
                const Spacer(),
                Row(
                  spacing: 10,
                  children: [
                    TopMenuIconItem(
                      icon: Icons.settings_outlined,
                      isSelected: 2 == _selected,
                      hasFocus: Focus.of(context).hasFocus,
                      onPressed: () {
                        Focus.of(context).requestFocus();
                        setSelected(2);
                        AppRouter.router.push(ScreenPaths.settings);
                      },
                    ),
                    TopMenuIconItem(
                      icon: Icons.notifications_none_outlined,
                      isSelected: 3 == _selected,
                      hasFocus: Focus.of(context).hasFocus,
                      onPressed: () {
                        Focus.of(context).requestFocus();
                        setSelected(3);
                        _showNotificationPanel();
                      },
                    ),
                    Text(
                      'TizenOS',
                      style: TextStyle(
                        color: Theme.of(context).colorScheme.onSurface,
                        fontSize: 15,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
