import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/review_card.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class ReviewList extends StatefulWidget {
  final VoidCallback? onFocused;
  final Function(int)? onSelectionChanged;
  final List<Reviews> reviews;

  const ReviewList({
    super.key,
    required this.reviews,
    this.onFocused,
    this.onSelectionChanged
  });

  @override
  State<ReviewList> createState() => _ReviewListState();
}

class _ReviewListState extends State<ReviewList> with FocusSelectable<ReviewList> {
  bool _hasFocus = false;
  late final int _itemCount;
  final double _listHeight = 110;

  @override
  void initState() {
    super.initState();
    focusNode.addListener(_onFocusChanged);
    _itemCount = widget.reviews.length;
  }

  @override
  void dispose() {
    focusNode.removeListener(_onFocusChanged);
    super.dispose();
  }

  void _onFocusChanged() async {
    setState(() {
      _hasFocus = focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      child: SizedBox(
        height: _listHeight,
        child: SelectableListView(
          key: listKey,
          padding: EdgeInsets.symmetric(horizontal: 58),
          itemCount: _itemCount,
          onSelectionChanged: (selectedIndex) => widget.onSelectionChanged?.call(selectedIndex),
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
