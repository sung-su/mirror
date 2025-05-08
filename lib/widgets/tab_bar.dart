import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/poc/setting_panel_poc.dart';

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
  final double _tabbarHeight = 80.0;
  final int _itemCount = 6;

  int _lastSelected = 0;
  int _selected = 0;
  
  @override
  void initState() {
    super.initState();
  }

  void movePage(int pageIndex) {
    debugPrint('_TvPageViewState._movePage: $_selected $pageIndex');
    widget.pageController.animateToPage(
      pageIndex,
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeInOut,
    );
  }

  void setSelected(int index) {
    debugPrint('[setSelected] selected=$_selected, _lastSelected=$_lastSelected, index=$index');
    if (_selected != index) {
      if(index == 5) {
        showSettingPanel(context);
      }
      else {
        setState(() {
          _selected = index;
        });
        _lastSelected = _selected;

        if(_selected > 0 && _selected < 4){
          movePage(_selected - 1);
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

  @override
  Widget build(BuildContext context) {
    debugPrint('_TvTabbarState.build() selected=$_selected, _lastSelected=$_lastSelected');
    return SizedBox(
      height: _tabbarHeight,
      child: Focus(
        autofocus: true,
        onFocusChange: (hasFocus) {
          debugPrint('[onFocusChange] selected=$_selected, _lastSelected=$_lastSelected');
          if(hasFocus) {
            setSelected(_lastSelected);
          } else{
            setState(() {
              _selected = -1;
            });
          }
        },
        onKeyEvent: (_, onKeyEvent) {
          if (onKeyEvent is KeyDownEvent) {
            if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowLeft) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.arrowLeft: $_selected');
              setSelected((_selected > 0 ) ? (_selected - 1) : _selected);
              return KeyEventResult.handled;
            }else if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowRight) {
              debugPrint('[onKeyEvent] LogicalKeyboardKey.arrowRight: $_selected');
              setSelected((_selected < _itemCount - 1) ? (_selected + 1) : _selected);
              return KeyEventResult.handled;
            } 
          }
          return KeyEventResult.ignored;
        },
        child: Padding (
          padding: const EdgeInsets.fromLTRB(16, 16, 0, 16),
            child: Row(
              spacing: 20,
              children: [
                TvAvatar(
                  imageUrl: null,
                  text: pages[0],
                  isSelected: 0 == _selected,
                ),
                TvTab(
                  text: pages[1],
                  isSelected: 1 == _selected,
                ),
                TvTab(
                  text: pages[2],
                  isSelected: 2 == _selected,
                ),
                TvTab(
                  text: pages[3],
                  isSelected: 3 == _selected,
                ),
                const Spacer(),
                TvTabIcon(
                  icon: const Icon(Icons.search, size: 18),
                  isSelected: 4 == _selected,
                ),
                TvTabIcon(
                  icon: const Icon(Icons.settings_outlined, size: 18),
                  isSelected: 5 == _selected,
                ),

                Text('TizenTV')
              ]
            ),
          ),
        )
      );
  }
}

class TvAvatar extends StatelessWidget {
  const TvAvatar({super.key, this.imageUrl, this.isSelected = false, this.text}); 
  final bool isSelected;
  final String? imageUrl;
  final String? text;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 40,
      height: 40,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        border: Border.all(
          color: isSelected ? Color.fromARGB(255, 125, 125, 125): Colors.transparent,
          width: 2.0,
        ),
      ),
      child : GestureDetector(
        onTap: () {
          // This will be executed when the CircleAvatar is pressed
          print('CircleAvatar was pressed!');
        },
        child: CircleAvatar(
          backgroundColor: Colors.brown.shade800,
          backgroundImage: imageUrl != null ? NetworkImage(imageUrl!) : null,
          child: (imageUrl == null && text != null) ? Text(text!) : null,
        )
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

class TvTabIcon extends StatelessWidget {
  const TvTabIcon({super.key, required this.icon, required this.isSelected}); 
  final bool isSelected;
  final Widget icon;

  @override
  Widget build(BuildContext context) {
    debugPrint('_TabButtonState.build()');
    return IconButton(
      icon: icon,
      onPressed: () {
        debugPrint('IconButton pressed');
      },
      style: IconButton.styleFrom(
        backgroundColor: isSelected ? Color.fromARGB(255, 125, 125, 125) : Colors.transparent
      ),
    );
  }
}