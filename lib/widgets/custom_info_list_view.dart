import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class CustomInfo {
  String name;
  Widget? popup;
  CustomInfo(this.name, {this.popup});
}

class CustomInfoListView extends StatefulWidget {
  const CustomInfoListView({
    super.key,
    required this.itemSource,
    this.onFocusChanged,
    this.onSelectionChanged,
  });

  final List<CustomInfo> itemSource;
  final Function(int)? onFocusChanged;
  final Function(int)? onSelectionChanged;

  @override
  State<CustomInfoListView> createState() => CustomInfoListViewState();
}

class CustomInfoListViewState extends State<CustomInfoListView>
    with FocusSelectable<CustomInfoListView> {
  int _selected = 0;

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
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
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
          _selected = listKey.currentState?.selectedIndex ?? 0;
        }
      },
      child: SelectableListView(
        scrollOffset: 260,
        key: listKey,
        //between item and item
        padding: const EdgeInsets.symmetric(vertical: 10),
        alignment: 0.5,
        itemCount: widget.itemSource.length,
        scrollDirection: Axis.vertical,
        onSelectionChanged: (selected) {
          _selected = selected;
          widget.onFocusChanged?.call(selected);
        },
        itemBuilder: (context, index, selectedIndex, key) {
          return AnimatedScale(
            key: key,
            scale:
                Focus.of(context).hasFocus && index == selectedIndex ? 1.0 : .9,
            duration: $style.times.med,
            curve: Curves.easeInOut,
            child: GestureDetector(
              onTap: () {
                listKey.currentState?.selectTo(index);
                Focus.of(context).requestFocus();
              },
              child: CustomItemView(
                name: widget.itemSource[index].name,
                isFocused: Focus.of(context).hasFocus && index == selectedIndex,
              ),
            ),
          );
        },
      ),
    );
  }
}

class CustomItemView extends StatelessWidget {
  const CustomItemView({
    super.key,
    required this.name,
    required this.isFocused,
  });

  final String name;
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
            spacing: 15,
            children: [
              // item text
              Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    name,
                    style: TextStyle(
                      fontSize: titleFontSize,
                      color:
                          isFocused
                              ? Theme.of(context).colorScheme.onTertiary
                              : Theme.of(context).colorScheme.tertiary,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
