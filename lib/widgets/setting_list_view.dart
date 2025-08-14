import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class SettingListView extends StatefulWidget{
  const SettingListView({super.key, required this.node, this.onSelectionChanged});

  final PageNode node;
  final Function(int)? onSelectionChanged;

  @override
  State<SettingListView> createState() => SettingListViewState();
}

class SettingListViewState extends State<SettingListView> with FocusSelectable<SettingListView> {

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
        itemCount: widget.node.children.length,
        scrollDirection: Axis.vertical,
        onSelectionChanged: (selected){
          widget.onSelectionChanged?.call(selected);
        },
        itemBuilder: (context, index, selectedIndex, key) {
          return AnimatedScale(
            key: key,
            scale: Focus.of(context).hasFocus && index == selectedIndex ? 1.0 : .9,
            duration: $style.times.med,
            curve: Curves.easeInOut,
            child: GestureDetector(
              onTap: () {
                listKey.currentState?.selectTo(index);
                Focus.of(context).requestFocus();
              },
              child: ItemView(index: index, selectedIndex: selectedIndex, node: widget.node)
            ),
          );
        }),
    );
  }
}

class ItemView extends StatelessWidget{
  const ItemView({super.key, required this.index, required this.selectedIndex, required this.node});

  final int index;
  final int selectedIndex;
  final PageNode node;

  final double titleFontSize = 13;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;
  final double iconSize = 25;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: node.children[index].description == null ? itemHeight : 82,//itemHeight * 1.25,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color:
              Focus.of(context).hasFocus && index == selectedIndex
                  ? Theme.of(context).colorScheme.tertiary
                  : Colors.transparent,
        ),
        child: Padding(
          padding: // left padding of item inside
              Focus.of(context).hasFocus && index == selectedIndex
                  ? EdgeInsets.symmetric(horizontal: 30)//innerPadding * 1.5)
                  : EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,//innerPadding * 0.75, // between icon-text spacing
            children: [
              if (node.children[index].icon != null)
                Container(
                  width: 43,//iconSize * 1.75,
                  height: 43,//iconSize * 1.75,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: Focus.of(context).hasFocus && index == selectedIndex
                        ? Color(0xF04285F4)
                        : Color(0xF0263041),
                  ),
                  child: Icon(
                      node.children[index].icon,
                      size: iconSize,
                      color: Focus.of(context).hasFocus && index == selectedIndex
                          ? Color(0xF0D3E0F5)
                          : Color(0xF0AEB2B9),
                    ),
                ),
              // item text
              Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    node.children[index].title,
                    style: TextStyle(
                      fontSize: titleFontSize,
                      color:
                          Focus.of(context).hasFocus && index == selectedIndex
                              ? Theme.of(context).colorScheme.onTertiary
                              : Theme.of(context).colorScheme.tertiary,
                    ),
                  ),
                  if (node.children[index].description != null)
                  Padding(
                    padding: // subtitle left padding
                      EdgeInsets.only(left: 10),
                    child :
                    Text(
                      node.children[index].description!,
                      style: TextStyle(
                        fontSize: subtitleFontSize,
                        color:Color(0xFF979AA0),
                      ),
                    ),
                  )
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
