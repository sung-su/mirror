import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/review_card.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class ReviewList extends StatefulWidget {
  final VoidCallback? onFocused;
  final List<Reviews> reviews;

  const ReviewList({
    super.key,
    required this.reviews,
    this.onFocused,
  });

  @override
  State<ReviewList> createState() => _ReviewListState();
}

class _ReviewListState extends State<ReviewList> {
  final FocusNode _focusNode = FocusNode();
  final GlobalKey<SelectableListViewState> _listViewKey =
      GlobalKey<SelectableListViewState>();

  bool _hasFocus = false;
  int _itemCount = 0;
  int _selectedIndex = 0;
  double _listHeight = 110;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
    _itemCount = widget.reviews.length;
    _selectedIndex = 0;
  }

  @override
  void dispose() {
    _focusNode.removeListener(_onFocusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _onFocusChanged() async {
    setState(() {
      _hasFocus = _focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call();
    }
  }

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowLeft &&
          _selectedIndex > 0) {
        _prev(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowRight &&
          _selectedIndex < _itemCount - 1) {
        _next(event is KeyRepeatEvent);
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }
  Future<void> _next(bool fast) async {
    if (_selectedIndex >= _itemCount - 1) {
      return;
    }
    var moved = await _listViewKey.currentState?.next(fast: fast);
    _selectedIndex = moved ?? _selectedIndex;
  }

  Future<void> _prev(bool fast) async {
    if (_selectedIndex <= 0) {
      return;
    }
    var moved = await _listViewKey.currentState?.previous(fast: fast);
    _selectedIndex = moved ?? _selectedIndex;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: SizedBox(
        height: _listHeight,
        child: SelectableListView(
          key: _listViewKey,
          padding: EdgeInsets.only(left: 58),
          itemCount: _itemCount,
          itemBuilder: (context, index, selectedIndex, key) {
            final review = widget.reviews[index];
            return Container(
                clipBehavior: Clip.none,
                margin: EdgeInsets.symmetric(horizontal: 15),
                child: ReviewCard(
                  key: key,
                  title: review.author,
                  description: review.content,
                  isSelected: Focus.of(context).hasFocus &&
                      index == selectedIndex,
                )
            );
          })
      ),
    );
  }
}
