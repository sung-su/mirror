import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class MockAppsPage extends StatefulWidget {
  MockAppsPage({super.key, required this.isHorizontal});

  bool isHorizontal = false;

  @override
  State<MockAppsPage> createState() => _MockAppsPageState();
}

class _MockAppsPageState extends State<MockAppsPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(null);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return widget.isHorizontal ? _buildHorizontalList(context) : _buildVerticalList(context);
  }

  Widget _buildHorizontalList(BuildContext context) {
    return Column(
      children: [
        MockList(isHorizontal: widget.isHorizontal),
        MockList(isHorizontal: widget.isHorizontal),
      ],
    );
  }

  Widget _buildVerticalList(BuildContext contact) {
    return Center(child: MockList(isHorizontal: widget.isHorizontal));
  }

}

class MockList extends StatefulWidget {
  MockList({super.key, required this.isHorizontal});

  bool isHorizontal = false;
  @override
  State<MockList> createState() => _MockListState();
}

class _MockListState extends State<MockList> with FocusSelectable<MockList> {
  final double itemSize = 300;

  @protected
  LogicalKeyboardKey getNextKey() {
    return widget.isHorizontal ? LogicalKeyboardKey.arrowRight : LogicalKeyboardKey.arrowDown;
  }

  @protected
  LogicalKeyboardKey getPrevKey() {
    return widget.isHorizontal ? LogicalKeyboardKey.arrowLeft : LogicalKeyboardKey.arrowUp;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      child: SizedBox(
        height: widget.isHorizontal ? itemSize * 0.6 : 400,
        child: Container(
          child: SelectableListView(
            key: listKey,
            padding: const EdgeInsets.symmetric(horizontal: 58, vertical: 5),
            alignment: 0.5,
            itemCount: 10,
            scrollDirection: widget.isHorizontal? Axis.horizontal : Axis.vertical,
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