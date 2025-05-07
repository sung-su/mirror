import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/poc/setting_panel_poc.dart';

class PageModel with ChangeNotifier {
  List<String> pages;
  int _index = 0;

  int get currentIndex => _index;
  int get pageCount => pages.length;

  PageModel(this.pages);

  String getPageName(int index) {
    return pages[index];
  }

  void select(int value) {
    _index = value;
    notifyListeners();
  }
}

class TvTabbar extends StatefulWidget {
  const TvTabbar({
    super.key,
  });

  @override
  State<TvTabbar> createState() => _TvTabbarState();
}

class _TvTabbarState extends State<TvTabbar> {
  final double _tabbarHeight = 80.0;

  int _selected = 0;
  int _focused = 0;
  int _itemCount = 0;

  @override
  void initState() {
    super.initState();
    _itemCount = context.read<PageModel>().pageCount;

  }

  void setSelected(int index) {
    debugPrint('[setSelected] selected=$_selected, focused=$_focused, index=$index');
    if (_selected != index) {
      setState(() {
        _selected = index;
      });
      _focused = (_selected >= 0) ? _selected : _focused;

      if(_selected > -1)
        context.read<PageModel>().select(_selected);
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

  @override
  Widget build(BuildContext context) {
    debugPrint('_TvTabbarState.build() selected=$_selected, focused=$_focused');
    return SizedBox(
      height: _tabbarHeight,
      child: Focus(
        autofocus: true,
        onFocusChange: (hasFocus) {
          debugPrint('[onFocusChange] selected=$_selected, focused=$_focused');
          if(hasFocus) {
            setSelected(_focused);
          } else{
            setSelected(-1);
          }
        },
        onKeyEvent: (_, onKeyEvent) {
          if (onKeyEvent is KeyDownEvent) {
            if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowLeft) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.arrowLeft');
              setSelected((_selected > 0 ) ? (_selected - 1) : _selected);
              return KeyEventResult.handled;
            }else if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowRight) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.arrowRight');
              if(_selected == _itemCount - 1) {
                debugPrint('[onKeyEvent] show setting panel');
                showSettingPanel(context);
              } else {
                setSelected((_selected < _itemCount - 1) ? (_selected + 1) : _selected);
              }
              return KeyEventResult.handled;
            } 
          }
          return KeyEventResult.ignored;
        },
        child: Padding (
          padding: const EdgeInsets.fromLTRB(16, 16, 0, 16),
            child: Row(
              spacing: 20,
              children: List.generate(_itemCount, (index) => Row(children: [
                TvTab(
                  text: context.read<PageModel>().getPageName(index),
                  isSelected: index == _selected,
                ),
              ])),
            ),
          ),
        )
      );
  }
}

class TvTab extends StatelessWidget {
  const TvTab({super.key, required this.text, required this.isSelected}); 
  final bool isSelected;
  final String text;

  @override
  Widget build(BuildContext context) {
    debugPrint('_TabButtonState.build()');
    return TextButton(
      onFocusChange: (hasFocus) {
        debugPrint('${text} focus changed to $hasFocus');
      },
      onPressed: () {
        debugPrint('TextButton pressed: ${text}');
      },
      style: TextButton.styleFrom(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(30),
        ),
        padding: const EdgeInsets.all(10),
        foregroundColor: isSelected ? Colors.black : const Color.fromARGB(255, 220, 220, 220) ,
        backgroundColor: isSelected ? const Color.fromARGB(255, 220, 220, 220)  : Colors.transparent,
      ),
      child: Text(
        text,
        style: TextStyle(fontSize: 16),
      ),
    );
  }
}