import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/bt_model.dart';
import 'package:tizen_fs/widgets/bt_selectable_listview.dart';
import 'package:tizen_fs/widgets/focus_selectable2.dart';

class BtDeviceListView extends StatefulWidget{
  const BtDeviceListView({super.key, this.onSelectionChanged, this.onAction});

  final Function(int)? onSelectionChanged;
  final Function(int)? onAction;

  @override
  State<BtDeviceListView> createState() => BtDeviceListViewState();
}

class BtDeviceListViewState extends State<BtDeviceListView> with FocusSelectable2<BtDeviceListView> {

  int _selected = 1;

  @protected
  LogicalKeyboardKey getNextKey() {
    return LogicalKeyboardKey.arrowDown;
  }

  @protected
  LogicalKeyboardKey getPrevKey() {
    return LogicalKeyboardKey.arrowUp;
  }

  void initFocus() {
    focusNode.requestFocus();
  }

  void selectTo(int index) {
    listKey.currentState?.selectTo(index);
  }

  void action(int index) {
    listKey.currentState?.onAction(index);
  }

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter || event.logicalKey == LogicalKeyboardKey.select) {
        action(_selected);
        return KeyEventResult.handled;
      } 
    }
    return KeyEventResult.ignored;
  }

  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      onFocusChange: (hasfocus) {
        if (hasfocus) {
          listKey.currentState?.selectTo(_selected);
        } else {
          _selected = listKey.currentState?.selectedIndex ?? 1;
        }
      },
      child: CategorySelectableListView(
        scrollOffset: 260,
        key: listKey,
        //between item and item
        padding: const EdgeInsets.symmetric(vertical: 10),
        alignment: 0.5,
        scrollDirection: Axis.vertical,
        onAction: widget.onAction,
        onSelectionChanged: (selected) {
          _selected = selected;
          // if(selected == 1) {
          //   widget.onSelectionChanged?.call(selected);  
          // }
        },
      ),
    );
  }
}