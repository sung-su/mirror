import 'package:flutter/material.dart';

class SelectableListView extends StatefulWidget {
  const SelectableListView({
    super.key,
    required this.itemCount,
    required this.itemBuilder,
    this.padding,
  });

  final int itemCount;
  final Widget Function(BuildContext, int index, int selectedIndex, Key key) itemBuilder;
  final EdgeInsets? padding;

  @override
  State<SelectableListView> createState() => SelectableListViewState();
}

class SelectableListViewState extends State<SelectableListView> {
  final ScrollController _controller = ScrollController();
  late List<GlobalKey> _itemKeys;

  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _itemKeys = List.generate(
      widget.itemCount,
      (index) => GlobalKey(),
    );
  }

  @override
  void didUpdateWidget(covariant SelectableListView oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.itemCount != widget.itemCount) {
      _itemKeys = List.generate(
        widget.itemCount,
        (index) => GlobalKey(),
      );
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    final int previousIndex = _selectedIndex;
    setState(() {
      _selectedIndex = index;
    });
    _scrollToSelected(_selectedIndex, previousIndex);
  }

  int get itemCount => widget.itemCount;

  Future<int> _scrollToSelected(int duration, int fallbackSelection) async {
    if (_itemKeys[_selectedIndex].currentContext != null) {
      int current = _selectedIndex;
      final RenderBox box = _itemKeys[_selectedIndex]
          .currentContext!
          .findRenderObject() as RenderBox;
      final Offset position = box.localToGlobal(Offset.zero);
      setState(() {});
      final double leftPadding = widget.padding?.left ?? 0;
      await _controller.animateTo(
        position.dx + _controller.offset - leftPadding,
        duration: Duration(milliseconds: duration),
        curve: Curves.easeInOut,
      );
      return current;
    } else {
      _selectedIndex = fallbackSelection; // restore previous selection
      return fallbackSelection;
    }
  }

  Future<int> selectTo(int index) {
    if (index < 0 || index >= widget.itemCount) {
      throw RangeError('Index out of range: $index');
    }
    final int previousIndex = _selectedIndex;
    _selectedIndex = index;
    return _scrollToSelected(100, previousIndex);
  }

  Future<int> next({bool fast = false}) async {
    if (_selectedIndex < widget.itemCount - 1) {
      final int previousIndex = _selectedIndex;
      _selectedIndex++;
      return await _scrollToSelected(fast ? 10 : 100, previousIndex);
    } else {
      return _selectedIndex;
    }
  }

  Future<int> previous({bool fast = false}) async {
    if (_selectedIndex > 0) {
      final int previousIndex = _selectedIndex;
      _selectedIndex--;
      return await _scrollToSelected(fast ? 10 : 100, previousIndex);
    } else {
      return _selectedIndex;
    }
  }

  @override
  Widget build(BuildContext context) {
    return ScrollConfiguration(
      behavior: ScrollBehavior().copyWith(scrollbars: false, overscroll: false),
      child: ListView.builder(
        padding: widget.padding,
        scrollDirection: Axis.horizontal,
        clipBehavior: Clip.none,
        controller: _controller,
        itemCount: widget.itemCount,
        itemBuilder: (context, index) {
          return widget.itemBuilder(
            context,
            index,
            _selectedIndex,
            _itemKeys[index],
          );
        },
      ),
    );
  }
}