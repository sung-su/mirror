import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/widgets/category_selectable_listview.dart';
import 'package:tizen_fs/widgets/focus_selectable2.dart';

class DeviceListItem {
  final String label;
  final bool isHeader;

  DeviceListItem.header(this.label) : isHeader = true;
  DeviceListItem.item(this.label) : isHeader = false;
}

class DeviceListView extends StatefulWidget{
  const DeviceListView({super.key, required this.itemSource, this.onSelectionChanged});

  final List<DeviceListItem> itemSource;
  final Function(int)? onSelectionChanged;

  @override
  State<DeviceListView> createState() => DeviceListViewState();
}

class DeviceListViewState extends State<DeviceListView> with FocusSelectable2<DeviceListView> {

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

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter || event.logicalKey == LogicalKeyboardKey.select) {
        debugPrint("enter: select ${_selected} !!!");
        widget.onSelectionChanged?.call(_selected);
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
        itemSource: widget.itemSource,
        scrollDirection: Axis.vertical,
        onSelectionChanged: (selected) {
          _selected = selected;
          // widget.onSelectionChanged?.call(selected);
        },
      ),
    );
  }
}

class DeviceItemListView extends StatelessWidget{
  const DeviceItemListView({super.key, required this.item, required this.isFocused});

  final DeviceListItem item;
  final bool isFocused;

  final double titleFontSize = 15;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;
  final double iconSize = 25;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: itemHeight,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color:
              isFocused
                  ? Theme.of(context).colorScheme.tertiary
                  : Colors.transparent,
        ),
        child: Padding(
          padding: // left padding of item inside
              EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,//innerPadding * 0.75, // between icon-text spacing
            children: [
              // if (item.icon != null)
              //   Container(
              //     width: 43,//iconSize * 1.75,
              //     height: 43,//iconSize * 1.75,
              //     decoration: BoxDecoration(
              //       shape: BoxShape.circle,
              //       color: isFocused
              //           ? Color(0xF04285F4).withAlphaF(0.2)
              //           : Color(0xF0263041),
              //     ),
              //     child: Icon(
              //         item.icon,
              //         size: iconSize,
              //         color: isFocused
              //             ? Color(0xF04285F4)
              //             : Color(0xF0AEB2B9),
              //       ),
              //   ),
              // item text
              Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.label,
                    style: TextStyle(
                      fontSize: titleFontSize,
                      color:
                          isFocused
                              ? Theme.of(context).colorScheme.onTertiary
                              : Theme.of(context).colorScheme.tertiary,
                    ),
                  ),
                  // if (item.description != null)
                  // Padding(
                  //   padding: EdgeInsets.only(left: 10),
                  //   child: Text(
                  //     item.description!,
                  //     style: TextStyle(
                  //       fontSize: subtitleFontSize,
                  //       color:Color(0xFF979AA0),
                  //     ),
                  //   ),
                  // )
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
