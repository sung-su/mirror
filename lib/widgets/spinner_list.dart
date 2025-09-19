import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class SpinnerList extends StatefulWidget {
  const SpinnerList({
    super.key,
    required this.items,
    this.initialIndex = 0,
    this.label,
    this.width = 140,
    this.height = 200,
    this.onChanged,
    this.onSubmitted,
  });

  final List<String> items;
  final int initialIndex;
  final String? label;
  final double width;
  final double height;
  final ValueChanged<int>? onChanged;
  final ValueChanged<int>? onSubmitted;

  @override
  State<SpinnerList> createState() => SpinnerListState();
}

class SpinnerListState extends State<SpinnerList>
    with FocusSelectable<SpinnerList> {
  int _selected = 0;
  int _focused = 0;

  @override
  void initState() {
    super.initState();
    _selected = widget.initialIndex.clamp(
      0,
      widget.items.isNotEmpty ? widget.items.length - 1 : 0,
    );
    _focused = _selected;
  }

  @override
  void didUpdateWidget(covariant SpinnerList oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.items.length != widget.items.length ||
        oldWidget.initialIndex != widget.initialIndex) {
      _selected = widget.initialIndex.clamp(
        0,
        widget.items.isNotEmpty ? widget.items.length - 1 : 0,
      );
      _focused = _selected;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) {
          listKey.currentState?.selectTo(_focused);
        }
      });
    }
  }

  @override
  LogicalKeyboardKey getNextKey() {
    return LogicalKeyboardKey.arrowDown;
  }

  @override
  LogicalKeyboardKey getPrevKey() {
    return LogicalKeyboardKey.arrowUp;
  }

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        _commitSelection(_focused);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  void _commitSelection(int index) {
    setState(() {
      _selected = index;
    });
    widget.onChanged?.call(index);
    widget.onSubmitted?.call(index);
  }

  @override
  Widget build(BuildContext context) {
    final bool hasFocus = focusNode.hasFocus;
    return SizedBox(
      width: widget.width,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (widget.label != null)
            Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: Text(
                widget.label!,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 13,
                  color: Theme.of(context).colorScheme.tertiary,
                ),
              ),
            ),
          Container(
            height: widget.height,
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(10),
              color: hasFocus
                  ? Theme.of(context).colorScheme.tertiary.withOpacity(0.15)
                  : Colors.transparent,
              border: Border.all(
                color: hasFocus
                    ? Theme.of(context).colorScheme.tertiary
                    : Colors.transparent,
                width: hasFocus ? 1 : 0,
              ),
            ),
            child: Focus(
              focusNode: focusNode,
              onFocusChange: (focused) {
                if (focused) {
                  listKey.currentState?.selectTo(_focused);
                } else {
                  _focused = listKey.currentState?.selectedIndex ?? _focused;
                }
              },
              child: SelectableListView(
                key: listKey,
                itemCount: widget.items.length,
                scrollDirection: Axis.vertical,
                alignment: 0.5,
                padding: const EdgeInsets.symmetric(vertical: 8),
                scrollOffset: widget.height / 2,
                onItemFocused: (index) {
                  _focused = index;
                },
                onItemSelected: (index) {
                  _focused = index;
                },
                itemBuilder: (context, index, selectedIndex, key) {
                  final bool selected =
                      focusNode.hasFocus && index == selectedIndex;
                  return AnimatedScale(
                    key: key,
                    scale: selected ? 1.0 : 0.92,
                    duration: $style.times.fast,
                    curve: Curves.easeInOut,
                    child: GestureDetector(
                      behavior: HitTestBehavior.opaque,
                      onTap: () {
                        listKey.currentState?.selectTo(index);
                        Focus.of(context).requestFocus();
                        _focused = index;
                        _commitSelection(index);
                      },
                      child: SizedBox(
                        height: 48,
                        child: Center(
                          child: Text(
                            widget.items[index],
                            style: TextStyle(
                              fontSize: selected ? 18 : 15,
                              color: selected
                                  ? Theme.of(context).colorScheme.onTertiary
                                  : Theme.of(context).colorScheme.tertiary,
                            ),
                          ),
                        ),
                      ),
                    ),
                  );
                },
              ),
            ),
          ),
        ],
      ),
    );
  }
}
