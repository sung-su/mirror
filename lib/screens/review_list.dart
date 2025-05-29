import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';

class MockReviewList extends StatefulWidget {
  final void Function(BuildContext)? onFocused;
  const MockReviewList({
    super.key,
    required this.reviews,
    this.onFocused,
  });
  final List<Reviews> reviews;

  @override
  State<MockReviewList> createState() => _MockReviewListState();
}

class _MockReviewListState extends State<MockReviewList> {
  final FocusNode _focusNode = FocusNode();
  bool _hasFocus = false;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
  }

  @override
  void dispose() {
    // _scrollController.dispose();
    _focusNode.removeListener(_onFocusChanged);
    _focusNode.dispose();
    super.dispose();
  }

  void _onFocusChanged() {
    setState(() {
      _hasFocus = _focusNode.hasFocus;
    });

    if (_hasFocus) {
      widget.onFocused?.call(context);
    }
  }

  void requestFocus()
  {
    _focusNode.requestFocus();
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      child: Container(
        padding: const EdgeInsets.only(left: 58, right: 58),
        // TODO: to be removed
        color : _hasFocus ? Colors.red : Colors.transparent,
        child: SingleChildScrollView(
          clipBehavior: Clip.none,
          scrollDirection: Axis.horizontal,
          child:
          Row(
            children: [
              ...List.generate(
                  widget.reviews.length,
                  (index) {
                    final review = widget.reviews[index];
                    return Card(
                      child: SizedBox(
                        height: 120,
                        width: 400,
                        child: Padding(
                          padding: const EdgeInsets.all(10),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(review.author, style: const TextStyle(fontSize: 15)),
                              const SizedBox(height: 5),
                              Text(review.content,
                                  maxLines: 3,
                                  overflow: TextOverflow.ellipsis,
                                  style: const TextStyle(fontSize: 12)),
                            ],
                          ),
                        ),
                      ),
                    );
                  }),
            ],
          )
        )
      ),
      onFocusChange: (hasFocus) {
        if(hasFocus) widget.onFocused?.call(context);
      }
    );
  }
}