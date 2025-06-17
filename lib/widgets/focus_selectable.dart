import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

mixin FocusSelectable<T extends StatefulWidget> on State<T> {
  final GlobalKey<SelectableListViewState> _listState =
      GlobalKey<SelectableListViewState>();
  final FocusNode _focusNode = FocusNode();
  int _selectedIndex = 0;

  GlobalKey<SelectableListViewState> get listKey => _listState;
  FocusNode get focusNode => _focusNode;
  int get selectedIndex => _selectedIndex;

  @override
  void initState() {
    super.initState();
    _focusNode.onKeyEvent = _onKeyEvent;
  }

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  @protected
  Future<void> onNext(bool fast) async {
    var moved = await _listState.currentState?.next(fast: fast);
    if (moved !=null && _selectedIndex != moved) {
      _selectedIndex = moved;
      _listState.currentState?.onSelectionChanged();
    }
  }

  @protected
  Future<void> onPrev(bool fast) async {
    var moved = await _listState.currentState?.previous(fast: fast);
    if (moved !=null && _selectedIndex != moved) {
      _selectedIndex = moved;
      _listState.currentState?.onSelectionChanged();
    }
  }

  @protected
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    return KeyEventResult.ignored;
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (onKeyEvent(focusNode, event) == KeyEventResult.handled) {
      return KeyEventResult.handled;
    }

    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        onPrev(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        onNext(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }
}
