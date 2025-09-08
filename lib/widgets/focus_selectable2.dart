import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/widgets/category_selectable_listview.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

mixin FocusSelectable2<T extends StatefulWidget> on State<T> {
  final GlobalKey<CategorySelectableListViewState> _listState =
      GlobalKey<CategorySelectableListViewState>();
  final FocusNode _focusNode = FocusNode();

  GlobalKey<CategorySelectableListViewState> get listKey => _listState;
  FocusNode get focusNode => _focusNode;
  int get selectedIndex => _listState.currentState?.selectedIndex ?? 0;

  LogicalKeyboardKey get nextKey => getNextKey();

  LogicalKeyboardKey get prevKey => getPrevKey();

  @protected
  LogicalKeyboardKey getNextKey() {
    return LogicalKeyboardKey.arrowRight;
  }

  @protected
  LogicalKeyboardKey getPrevKey() {
    return LogicalKeyboardKey.arrowLeft;
  }

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
    await _listState.currentState?.next(fast: fast);
  }

  @protected
  Future<void> onPrev(bool fast) async {
    await _listState.currentState?.previous(fast: fast);
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
      if (event.logicalKey == prevKey) {
        onPrev(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == nextKey) {
        onNext(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }
}
