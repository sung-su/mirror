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
        scrollOffset: 275,
        key: listKey,
        padding: const EdgeInsets.symmetric(horizontal: 25),
        alignment: 0.5,
        itemCount: widget.node.children.length,
        scrollDirection: Axis.vertical,
        onSelectionChanged: (selected){
          widget.onSelectionChanged?.call(selected);
        },
        itemBuilder: (context, index, selectedIndex, key) {
          return AnimatedScale(
            key: key,
            scale: Focus.of(context).hasFocus && index == selectedIndex ? 1.1 : 1.0,
            duration: $style.times.fast,
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

  final double titleFontSize = 15;
  final double subtitleFontSize = 13;
  final double innerPadding = 20;
  final double itemHeight = 60;
  final double iconSize = 25;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: node.children[index].description == null ? itemHeight : itemHeight * 1.25,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color: Focus.of(context).hasFocus && index == selectedIndex ? Theme.of(context).colorScheme.tertiary : Colors.transparent,
        ),
        child: Padding(
          padding:
              Focus.of(context).hasFocus && index == selectedIndex
                  ? EdgeInsets.symmetric(horizontal: innerPadding * 1.25)
                  : EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: innerPadding,
            children: [
              if (node.children[index].icon != null)
                Container(
                  width: iconSize * 2,
                  height: iconSize * 2,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: Focus.of(context).hasFocus && index == selectedIndex
                        ? Colors.blueAccent
                        : Colors.grey[800],
                  ),
                  child: Icon(
                      node.children[index].icon,
                      size: iconSize,
                      color: Focus.of(context).hasFocus && index == selectedIndex
                          ? Colors.white
                          : Colors.grey[600],
                    ),
                ),
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
                  Padding( //subtitle left side
                    padding: EdgeInsets.only(left: innerPadding / 2),
                    child :
                    Text(
                      node.children[index].description!,
                      style: TextStyle(
                        fontSize: subtitleFontSize,
                        color:
                            Focus.of(context).hasFocus && index == selectedIndex
                                ? Theme.of(context).colorScheme.onTertiary.withAlpha(175)
                                : Theme.of(context).colorScheme.tertiary.withAlpha(175),
                      ),
                    ),
                  )
                ],
              ),
            ],
          ),
        )
      ),
    );
  }
}
