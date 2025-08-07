import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';

class InfiniteScollList extends StatefulWidget {
  const InfiniteScollList({super.key});

  @override
  State<InfiniteScollList> createState() => _InfiniteScollListState();
}

class _InfiniteScollListState extends State<InfiniteScollList> {
  final ScrollController _controller = ScrollController();
  final FocusNode _listFocusNode = FocusNode();
  final double _itemWidth = 200;
  final int _fetchThreshold = 100;

  List<int> _items = List.generate(21, (i) => i - 10); // [-10, ..., 10]
  List<GlobalKey> _itemKeys = List.generate(21, (_) => GlobalKey());

  int _focusedIndex = 10;

  @override
  void initState() {
    super.initState();

    _controller.addListener(() {
      if (_controller.offset < _fetchThreshold) {
        _prependItems();
      } else if (_controller.position.maxScrollExtent - _controller.offset < _fetchThreshold) {
        _appendItems();
      }
    });

    WidgetsBinding.instance.addPostFrameCallback((_) {
      final middleOffset = _focusedIndex * _itemWidth - 500;
      _controller.jumpTo(middleOffset);
      _requestFocus(_focusedIndex);
    });
  }

  void _prependItems() {
    final first = _items.first;
    final newItems = List.generate(10, (i) => first - 10 + i);
    final newKeys = List.generate(10, (_) => GlobalKey());

    setState(() {
      _items = [...newItems, ..._items];
      _itemKeys = [...newKeys, ..._itemKeys];
      _focusedIndex += newItems.length;
    });


    final middleOffset = _focusedIndex * _itemWidth - 500;

    _controller.jumpTo(middleOffset);
  }

  void _appendItems() {
    final last = _items.last;
    final newItems = List.generate(10, (i) => last + i + 1);
    final newKeys = List.generate(10, (_) => GlobalKey());

    setState(() {
      _items.addAll(newItems);
      _itemKeys.addAll(newKeys);
    });
  }

  void _requestFocus(int index) {
    final key = _itemKeys[index];
    final context = key.currentContext;
    if (context != null) {
      _scrollToIndex2(index, context);
      setState(() {
        _focusedIndex = index;  
      }); 
    }
  }

  void _scrollToIndex(int index) {
    final offset = index * _itemWidth;
    _controller.animateTo(
      offset,
      duration: const Duration(milliseconds: 200),
      curve: Curves.easeInOut,
    );
  }

  void _scrollToIndex2(int index, BuildContext context) {
    Scrollable.ensureVisible(
      context,
      alignment: 0.5,
      duration: $style.times.fast,
      curve: Curves.easeInOut,
    );
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        if (_focusedIndex == _items.length - 1) {
          _appendItems();
        }
        _requestFocus(_focusedIndex + 1);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        if (_focusedIndex == 0) {
          _prependItems();
        }
        _requestFocus(_focusedIndex - 1);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        title: const Text('Infinite List Test'),
      ),
      body: Focus(
        focusNode: _listFocusNode,
        autofocus: true,
        onKeyEvent: _handleKey,
        child: ListView.builder(
          controller: _controller,
          scrollDirection: Axis.horizontal,
          itemExtent: _itemWidth,
          itemCount: _items.length,
          itemBuilder: (context, index) {
            return Container(
              key: _itemKeys[index],
              child: Builder(builder: (context,) {
                return Container(
                  margin: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: (_focusedIndex == index) ? Colors.grey.shade900 : Colors.grey.shade800,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color: (_focusedIndex == index) ? Colors.red : Colors.transparent,
                      width: 2,
                    ),
                  ),
                  width: _itemWidth,
                  child: Center(
                    child: Text(
                      'Item ${_items[index]}',
                      style: const TextStyle(fontSize: 16),
                    ),
                  ),
                );
              }),
            );
          },
        ),
      ),
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    _listFocusNode.dispose();
    super.dispose();
  }
}