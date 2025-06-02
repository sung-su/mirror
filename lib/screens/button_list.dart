import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';

class ButtonList extends StatefulWidget {
  final VoidCallback? onFocused;
  const ButtonList({
    super.key,
    this.onFocused,
  });

  @override
  State<ButtonList> createState() => ButtonListState();
}
class ButtonListState extends State<ButtonList> {
  final FocusNode _focusNode = FocusNode();

  bool _hasFocus = false;
  int _itemCount = 5;
  int _selectedIndex = 0;

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
    }
  }

  void requestFocus(){
    _focusNode.requestFocus();
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          _selectedIndex > 0) {
        _prev(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {
        _next(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
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
    });
  }

  void _prev(bool fast) {
    if (_selectedIndex <= 0) {
      return;
    }
    setState((){
      _selectedIndex--;
    });
  }

  Widget _buildButton(int itemIndex, String? text, IconData? iconData) {
    if (text == null)
      return SizedBox(
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
      );

    return ElevatedButton.icon(
      onPressed: () {},
      onFocusChange: (hasFocus) {
      },
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
      child: Container(
        padding: EdgeInsets.only(left: 58),
        child: Row(
          spacing: 10,
          children: [
            _blurEffect(_buildButton(0, 'Trailer', Icons.live_tv_rounded)),
            _blurEffect(_buildButton(1, 'Buy  \$10.0', null)),
            _blurEffect(_buildButton(2, 'Watchlist', Icons.bookmark_border_outlined)),
            _blurEffect(_buildButton(3, null, (_selectedIndex < 3) ? Icons.thumbs_up_down_sharp : Icons.thumb_up_sharp)),
            if(_selectedIndex >= 3) 
              _blurEffect(_buildButton(4, null, Icons.thumb_down_sharp))
          ]
        )
      ),
    );
  }
}