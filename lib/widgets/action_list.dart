import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/screens/video_player_page.dart';
import 'package:tizen_fs/styles/app_style.dart';

class ActionList extends StatefulWidget {
  const ActionList({
    super.key,
    required this.movie,
    this.onFocused,
    this.onSelectionChanged
  });

  final Movie movie;
  final VoidCallback? onFocused;
  final Function(int)? onSelectionChanged;
  
  @override
  State<ActionList> createState() => ActionListState();
}
class ActionListState extends State<ActionList> {
  final FocusNode _focusNode = FocusNode();

  bool _hasFocus = false;
  bool _isEnabled = true;
  int _itemCount = 5;
  int _selectedIndex = 0;
  Map<int, VoidCallback> _actions = Map<int, VoidCallback>()  ;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
    _selectedIndex = 0;
  }

  @override
  void dispose() {
    _focusNode.removeListener(_onFocusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _onFocusChanged() async {
    setState(() {
      _hasFocus = _focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call();
      enable(true);
    }
  }

  void requestFocus(){
    _focusNode.requestFocus();
    enable(true);
  }

  void enable(bool enable) {
    setState(() {
      _isEnabled = enable;
    });
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        _prev(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        _next(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
          _actions[_selectedIndex]?.call();
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  void _next(bool fast) {
    if (_selectedIndex >= _itemCount - 1) {
      return;
    }
    setState((){
      _selectedIndex++;
      widget.onSelectionChanged?.call(_selectedIndex);
    });
  }

  void _prev(bool fast) {
    if (_selectedIndex <= 0) {
      return;
    }
    setState((){
      _selectedIndex--;
      widget.onSelectionChanged?.call(_selectedIndex);
    });
  }

  Widget _buildButton(int itemIndex, String? text, IconData? iconData, VoidCallback? callback) {
    _actions[itemIndex] = callback ?? (){};

    if (text == null) {
      return AnimatedScale(
      duration: Duration(milliseconds: 200),
      scale: (_hasFocus && itemIndex == _selectedIndex) ? 1.1 : 1,
        child: SizedBox(
          width: 50,
          child: IconButton.filled(
            onPressed: () {},
            icon: Icon(
              iconData,
              size: 15,
            ),
            style : FilledButton.styleFrom(
              animationDuration: Duration(milliseconds: 0),
              backgroundColor: (_hasFocus && itemIndex == _selectedIndex) ? Colors.white : Colors.grey.withAlphaF(0.4),
              foregroundColor: (_hasFocus && itemIndex == _selectedIndex) ? Colors.black.withAlphaF(0.8) : Colors.white.withAlphaF(0.8),
            )
          ),
        ),
      );
    }

    return AnimatedScale(
      duration: Duration(milliseconds: 200),
      scale: (_hasFocus && itemIndex == _selectedIndex) ? 1.1 : 1,
      child: ElevatedButton.icon(
        onPressed: () {},
        icon: iconData != null 
          ? Icon(
            iconData,
            size: 17,
            color:(_hasFocus && itemIndex == _selectedIndex) ? Colors.black.withAlphaF(0.8) : Colors.white.withAlphaF(0.8)
          ) : null,
        label: Text(
          text,
        ),
        style : ElevatedButton.styleFrom(
          animationDuration: Duration(milliseconds: 0),
          backgroundColor: (_hasFocus && itemIndex == _selectedIndex) ? Colors.white : Colors.grey.withAlphaF(0.4),
          foregroundColor: (_hasFocus && itemIndex == _selectedIndex) ? Colors.black.withAlphaF(0.8) : Colors.white.withAlphaF(0.8),
        )
      ),
    );
  }

  Widget _blurEffect(Widget child)
  {
    // // TODO: performance issue when using blur effect
    // return ClipRRect(
    //   borderRadius: BorderRadius.circular(25),
    //   child: BackdropFilter(
    //     filter: ImageFilter.blur(sigmaX: 5, sigmaY: 5),
    //     child: child
    //   )
    // );

    return child;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: AnimatedOpacity(
        opacity: _isEnabled ? 1.0 : 0.6,
        duration: const Duration(milliseconds: 300),
        child: Container(
          padding: EdgeInsets.only(left: 58),
          child: Row(
            spacing: 10,
            children: [
              _blurEffect(_buildButton(0, 'Trailer', Icons.live_tv_rounded, () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => VideoPlayerPage(title: widget.movie.title, videoUrl: widget.movie.backdropVideoUrl),
                  ));
              })),
              _blurEffect(_buildButton(1, 'Buy  \$10.0', null, null)),
              _blurEffect(_buildButton(2, 'Watchlist', Icons.bookmark_border_outlined, null)),
              _blurEffect(_buildButton(3, null, (_selectedIndex < 3) ? Icons.thumbs_up_down_sharp : Icons.thumb_up_sharp, null)),
              if(_selectedIndex >= 3)
                _blurEffect(_buildButton(4, null, Icons.thumb_down_sharp, null))
            ]
          )
        ),
      ),
    );
  }
}