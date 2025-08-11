import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class ListView extends StatefulWidget{
  const ListView({super.key, required this.onSelectionChanged});

  final Function(int) onSelectionChanged;

  @override
  State<ListView> createState() => _ListViewState();
}

class _ListViewState extends State<ListView> with FocusSelectable<ListView>  {

  double itemSize = 300;

  @protected
  LogicalKeyboardKey getNextKey() {
    return LogicalKeyboardKey.arrowDown;
  }

  @protected
  LogicalKeyboardKey getPrevKey() {
    return LogicalKeyboardKey.arrowUp;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      child: SizedBox(
        height: itemSize,
        child: Container(
          child: SelectableListView(
            key: listKey,
            padding: const EdgeInsets.symmetric(horizontal: 58, vertical: 5),
            alignment: 0.5,
            itemCount: 10,
            scrollDirection: Axis.vertical,
            onSelectionChanged: widget.onSelectionChanged,
            itemBuilder: (context, index, selectedIndex, key) {
              return AnimatedScale(
                key: key,
                scale:
                    Focus.of(context).hasFocus && index == selectedIndex ? 1.1 : 1.0,
                duration: const Duration(milliseconds: 200),
              child: GestureDetector(
                onTap: () {
                  listKey.currentState?.selectTo(index);
                  Focus.of(context).requestFocus();
                },
                child: Card(
                  margin: EdgeInsets.all(10),
                  shape: Focus.of(context).hasFocus && index == selectedIndex
                      ? RoundedRectangleBorder(
                          side: BorderSide(color: Colors.grey.withAlphaF(0.5), width: 2.0),
                          borderRadius: BorderRadius.circular(10),
                        )
                      : null,
                    child: SizedBox(
                      width: itemSize,
                      height: 50,
                      child: Container(
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(10),
                          color: Theme.of(context).colorScheme.onTertiary,
                        ),
                      ),
                    ),
                  ),
              ),
              );
            }),
        ),
      ),
    );
  }
}