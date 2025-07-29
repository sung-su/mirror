import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/widgets/top_menu_avatar_item.dart';
import 'package:tizen_fs/widgets/top_menu_button_item.dart';
import 'package:tizen_fs/widgets/top_menu_icon_item.dart';
import 'account_panel.dart';

class HomeTopMenu extends StatefulWidget {
  const HomeTopMenu({
    super.key,
    required this.pageController,
  });

  final PageController pageController;

  @override
  State<HomeTopMenu> createState() => _HomeTopMenuState();
}

class _HomeTopMenuState extends State<HomeTopMenu> {
  final List<String> pages = ['QA', 'Home', 'Media', 'IoT'];
  final int _itemCount = 6;

  int _selected = 1;

  @override
  void initState() {
    super.initState();
  }

  void _movePage(int pageIndex) {
    debugPrint('_TvPageViewState._movePage: $_selected $pageIndex');
    widget.pageController.animateToPage(
      pageIndex,
      duration: const Duration(milliseconds: 150),
      curve: Curves.easeInOut,
    );
  }

  void setSelected(int index) {
    debugPrint('[setSelected] selected=$_selected, index=$index');
    if (_selected != index) {
      setState(() {
        _selected = index;
      });

      if (_selected > 0 && _selected < 5) {
        _movePage(_selected - 1);
      } else if (_selected == 0) {
        showAccountPanel();
      }
    }
  }

  void showAccountPanel() {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Close",
      barrierColor: Colors.transparent,
      transitionDuration: const Duration(milliseconds: 80),
      pageBuilder: (BuildContext buildContext, Animation animation,
          Animation secondaryAnimation) {
            return AccountPanel();
          },
        transitionBuilder: (context, animation, secondaryAnimation, child) {
          return FadeTransition(
            opacity: animation,
            child: child,
          );
        },
    );
  }

  @override
  Widget build(BuildContext context) {
    debugPrint('_TvTabbarState.build() selected=$_selected');
    return Focus(
      autofocus: true,
      onFocusChange: _onFocusChanged,
      onKeyEvent: (_, onKeyEvent) {
        if (onKeyEvent is KeyDownEvent) {
          debugPrint(
              '[onKeyEvent] LogicalKeyboardKey ${onKeyEvent.logicalKey}');
          if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowLeft) {
            debugPrint('[onKeyEvent] LogicalKeyboardKey.arrowLeft: $_selected');
            setSelected((_selected > 0) ? (_selected - 1) : _selected);
            return KeyEventResult.handled;
          } else if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowRight) {
            debugPrint(
                '[onKeyEvent] LogicalKeyboardKey.arrowRight: $_selected');
            setSelected(
                (_selected < _itemCount - 1) ? (_selected + 1) : _selected);
            return KeyEventResult.handled;
          } else if (onKeyEvent.logicalKey == LogicalKeyboardKey.enter) {
            debugPrint('[onKeyEvent] LogicalKeyboardKey.enter: $_selected');
            if (_selected == 0) {
              AppRouter.router.go(ScreenPaths.poc);
            }
            return KeyEventResult.handled;
          }
        }
        return KeyEventResult.ignored;
      },
      child: Builder(builder: (context) {
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
              SizedBox(
                width: 15,
              ),
              TopMenuButtonItem(
                  text: pages[1],
                  isSelected: 1 == _selected,
                  isFocused: Focus.of(context).hasFocus,
                  onPressed: () {
                    Focus.of(context).requestFocus();
                    setSelected(1);
                  }),
              TopMenuButtonItem(
                  text: pages[2],
                  isSelected: 2 == _selected,
                  isFocused: Focus.of(context).hasFocus,
                  onPressed: () {
                    Focus.of(context).requestFocus();
                    setSelected(2);
                  }),
              TopMenuButtonItem(
                  text: pages[3],
                  isSelected: 3 == _selected,
                  isFocused: Focus.of(context).hasFocus,
                  onPressed: () {
                    Focus.of(context).requestFocus();
                    setSelected(3);
                  }),
              const Spacer(),
              Row(
                spacing: 10,
                children: [
                  TopMenuIconItem(
                    icon: Icons.settings_outlined,
                    isSelected: 4 == _selected,
                    hasFocus: Focus.of(context).hasFocus,
                    onPressed: () {
                      Focus.of(context).requestFocus();
                      setSelected(4);
                    }
                  ),
                  TopMenuIconItem(
                    icon: Icons.notifications_none_outlined,
                    isSelected: 5 == _selected,
                    hasFocus: Focus.of(context).hasFocus,
                    onPressed: () {
                      Focus.of(context).requestFocus();
                      setSelected(5);
                    }
                  ),
                  Text(
                    'TizenOS',
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.w600
                    ),
                  )
                ],
              )
            ]),
        );
      }),
    );
  }

  void _onFocusChanged(hasFocus) {
      if (hasFocus) {
        if (_selected == 0) {
          setState(() {
            _selected = 1;
          });
        }
      }
      Provider.of<BackdropProvider>(context, listen: false).isZoomIn = hasFocus;
    }
}
