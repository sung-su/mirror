import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/page_node.dart';
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
          debugPrint("######################### ${widget.node.title} listview focused _selected=$_selected");
          listKey.currentState?.selectTo(_selected);
        } else {
          debugPrint("######################### ${widget.node.title} listview unfocused _selected=$_selected");
          _selected = listKey.currentState?.selectedIndex ?? 0;
        }
      },
      child: SelectableListView(
        key: listKey,
        padding: const EdgeInsets.symmetric(horizontal: 25, vertical: 5),
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
            duration: const Duration(milliseconds: 200),
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

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 60,
      child: Container(
        // margin: const EdgeInsets.all(5),
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color: Focus.of(context).hasFocus && index == selectedIndex ? Theme.of(context).colorScheme.tertiary : Colors.transparent,
        ),
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 5),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,
            children: [
              if (node.children[index].icon != null) node.children[index].icon!,
              Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                spacing: 3,
                children: [
                  Text(
                    node.children[index].title,
                    style: TextStyle(
                      fontSize: 15,
                      color: Focus.of(context).hasFocus && index == selectedIndex ? Theme.of(context).colorScheme.onTertiary : Theme.of(context).colorScheme.tertiary,
                    )
                  ),
                  if (node.children[index].description != null) 
                  Text(
                    node.children[index].description!,
                    style: TextStyle(
                      fontSize: 12,
                      color: Focus.of(context).hasFocus && index == selectedIndex ? Theme.of(context).colorScheme.onTertiary : Theme.of(context).colorScheme.tertiary,
                    )
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