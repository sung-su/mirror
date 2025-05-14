import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/main.dart';
import 'package:tizen_fs/poc/setting_panel_poc.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'account_panel.dart';

class TvTabbar extends StatefulWidget {
  const TvTabbar({
    super.key,
    required this.pageController,
  });

  final PageController pageController;

  @override
  State<TvTabbar> createState() => _TvTabbarState();
}

class _TvTabbarState extends State<TvTabbar> {
  final List<String> pages = ['QA', 'Home', 'Apps', 'Library'];
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
      if (index == 5) {
        showSettingPanel(context);
      } else {
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
  }

  void showSettingPanel(BuildContext context) {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Close",
      barrierColor: Colors.transparent,
      transitionDuration: const Duration(milliseconds: 200),
      pageBuilder: (BuildContext buildContext, Animation animation,
          Animation secondaryAnimation) {
        return SettingPanel();
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return SlideTransition(
          position: Tween<Offset>(
            begin: const Offset(1, 0),
            end: Offset.zero,
          ).animate(animation),
          child: FadeTransition(
            opacity: animation,
            child: child,
          ),
        );
      },
    );
  }

  void showAccountPanel() {
    // final FocusNode accountFocusNode = FocusNode();
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
          padding: const EdgeInsets.fromLTRB(48, 20, 48, 0),
          child: Row(
            children: [
              TvAvatar(
                imageUrl: null,
                text: pages[0],
                isSelected: 0 == _selected,
              ),
              SizedBox(
                width: 10,
              ),
              TvTab(
                  text: pages[1],
                  isSelected: 1 == _selected,
                  isFocused: Focus.of(context).hasFocus),
              TvTab(
                  text: pages[2],
                  isSelected: 2 == _selected,
                  isFocused: Focus.of(context).hasFocus),
              TvTab(
                  text: pages[3],
                  isSelected: 3 == _selected,
                  isFocused: Focus.of(context).hasFocus),
              const Spacer(),
              Row(
                spacing: 10,
                children: [
                  TvTabIcon(
                    icon: Icons.search,
                    isSelected: 4 == _selected,
                    hasFocus: Focus.of(context).hasFocus,
                  ),
                  TvTabIcon(
                    icon: Icons.settings_outlined,
                    isSelected: 5 == _selected,
                    hasFocus: Focus.of(context).hasFocus,
                  ),
                  Text(
                    'TizenTV',
                    style: TextStyle(fontSize: 15),
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
    }
}

class TvAvatar extends StatelessWidget {
  const TvAvatar(
      {super.key, this.imageUrl, this.isSelected = false, this.text});
  final bool isSelected;
  final String? imageUrl;
  final String? text;

  @override
  Widget build(BuildContext context) {
    return Container(
        width: 30,
        height: 30,
        decoration: BoxDecoration(
          shape: BoxShape.circle,
          boxShadow:[
            BoxShadow(
              color: isSelected ? $style.colors.onSurface : Colors.transparent,
              spreadRadius: 2,
            )
          ]
        ),
        child: GestureDetector(
            onTap: () {
              // This will be executed when the CircleAvatar is pressed
              print('CircleAvatar was pressed!');
            },
            child: CircleAvatar(
              backgroundColor: Colors.brown.shade800,
              backgroundImage:
                  imageUrl != null ? NetworkImage(imageUrl!) : null,
              child: (imageUrl == null && text != null) ? Text(text!) : null,
            )));
  }
}

class TvTab extends StatelessWidget {
  const TvTab({super.key,
      required this.text,
      required this.isSelected,
      required this.isFocused});
  final bool isSelected;
  final bool isFocused;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: isSelected
            ? (isFocused
                ? Colors.white.withAlphaF(0.9)
                : Colors.white.withAlphaF(0.15))
            : Colors.transparent,
        borderRadius: isSelected ? BorderRadius.circular(30) : BorderRadius.zero,
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 17, vertical: 10),
        child: Text(
          text,
          style: TextStyle(
            fontSize: 16,
            color: isFocused
              ? (isSelected
                  ? $style.colors.surface
                  : $style.colors.onSurface)
              : $style.colors.onSurface.withAlphaF(0.8),
          ),
        ),
      ),
    );
  }
}

class TvTabIcon extends StatelessWidget {
  const TvTabIcon(
      {super.key,
      required this.icon,
      required this.isSelected,
      required this.hasFocus});
  final bool isSelected;
  final IconData icon;
  final bool hasFocus;

  @override
  Widget build(BuildContext context) {
    debugPrint('_TabButtonState.build()');
    return Container(
      height: 30,
      width: 30,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        boxShadow: [
          BoxShadow(
            color: isSelected ? $style.colors.onSurface : Colors.transparent,
            spreadRadius: 1,
          )
        ]
      ),
      child: IconButton(
        padding: EdgeInsets.all(0.0),
        icon: Icon(
          icon,
          size: 17,
          color: isSelected ? $style.colors.surface : $style.colors.onSurface,
        ),
        onPressed: () {
          debugPrint('IconButton pressed');
        },
        style: IconButton.styleFrom(
            backgroundColor: isSelected
                ? (hasFocus
                    ? $style.colors.onPrimaryContainer.withAlphaF(0.8)
                    : $style.colors.onPrimaryContainer.withAlphaF(0.3))
                : $style.colors.onPrimaryContainer.withAlphaF(0.3)),
      ),
    );
  }
}
