import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';

class IfYouLikeList extends StatefulWidget {
  final void Function(BuildContext)? onFocused;
  const IfYouLikeList({
    super.key,
    this.onFocused,
  });

  @override
  State<IfYouLikeList> createState() => IfYouLikeListState();
}

class IfYouLikeListState extends State<IfYouLikeList> {
  final FocusNode _focusNode = FocusNode();
  final _itemCount = 10;
  bool _hasFocus = false;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
  }

  @override
  void dispose() {
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

  @override
    @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Container(
        padding: const EdgeInsets.only(left: 58, right: 58),
        //TODO: to be removed
        color : _hasFocus ? Colors.red : Colors.transparent, 
        child: SingleChildScrollView(
          clipBehavior: Clip.none,
          scrollDirection: Axis.horizontal,
          child: Row(
            spacing: 10,
            children: [
              ...List.generate(
                  _itemCount,
                  (index) => Card.outlined(
                    child: SizedBox(
                  height: 150,
                  width: 200,
                ))),
          ]),
        )),
    );
  }
}