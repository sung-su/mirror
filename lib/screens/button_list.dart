import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/styles/app_style.dart';

class ButtonList extends StatefulWidget {
  final void Function(BuildContext)? onFocused;
  const ButtonList({
    super.key,
    this.onFocused,
  });

  @override
  State<ButtonList> createState() => ButtonListState();
}

class ButtonListState extends State<ButtonList> {
  final FocusNode _focusNode = FocusNode();
  final _itemCount = 3;
  bool _hasFocus = false;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
  }

  @override
  void dispose() {
    // _scrollController.dispose();
    _focusNode.removeListener(_onFocusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _onFocusChanged() {
    setState(() {
      _hasFocus = _focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call(context);
    }
  }

  void requestFocus()
  {
    _focusNode.requestFocus();
  }

  
  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          _selectedIndex > 0) {

        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {

        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  Widget _buildButton(String? text, IconData? iconData) {
    if (text == null)
    return IconButton.filled(
      onPressed: () {},
      icon: Icon(iconData),
      style : FilledButton.styleFrom(
        backgroundColor: Colors.grey.withAlphaF(0.4)
      )
    );

    return FilledButton.icon(
      onPressed: () {},
      onFocusChange: (hasFocus) {
      },
      icon: iconData != null ? Icon(iconData, size: 17) : null,
      label: Text(
        text,
        style: TextStyle(
          color: Colors.white.withAlphaF(0.8),
        )
      ),
      style : FilledButton.styleFrom(
        backgroundColor: Colors.grey.withAlphaF(0.4),
      )
    );
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Container(
        // TODO: to be removed
        color : _hasFocus ? Colors.red : Colors.transparent, 
        padding: const EdgeInsets.only(left: 58, right: 58),
        child: SingleChildScrollView(
          clipBehavior: Clip.none,
          scrollDirection: Axis.horizontal,
          child: Row(
            spacing: 10, 
            children: [
              _buildButton('Trailer', Icons.live_tv_rounded),
              _buildButton('Buy  \$10.0', null),
              _buildButton('Watchlist', Icons.bookmark_border_outlined),
              _buildButton(null, Icons.format_quote_outlined),
            ]),
        )),
    );
  }
}

