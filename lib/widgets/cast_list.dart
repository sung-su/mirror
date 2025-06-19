import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class CastList extends StatefulWidget {
  final VoidCallback? onFocused;
  final List<Cast> casts;
  final String title;

  const CastList({
    super.key,
    required this.casts,
    this.onFocused,
    this.title = '',
  });

  @override
  State<CastList> createState() => _CastListState();
}

class _CastListState extends State<CastList> with FocusSelectable<CastList> {
  late final String _title;

  bool _hasFocus = false;
  late final int _itemCount;
  final double _titleFontSize = 14; // * 1.7
  final double _listExtenedHeight = 170;
  final double _listHeight = 130;

  @override
  void initState() {
    super.initState();
    focusNode.addListener(_onFocusChanged);
    _itemCount = widget.casts.length;
    _title = widget.title;
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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          //list title
          Padding(
            padding: const EdgeInsets.fromLTRB(58, 0, 70, 8),
            child: SizedBox(
              height: _hasFocus ? 40 : 20,
              child: AnimatedScale(
                  scale: _hasFocus ? 1.7 : 1.0,
                  duration: const Duration(milliseconds: 100),
                  alignment: Alignment.topLeft,
                  child: Text(
                    _title,
                    textAlign: TextAlign.left,
                    style: TextStyle(
                      fontSize: _titleFontSize,
                      color: _hasFocus
                          ? Colors.white
                              .withAlpha((255 * 0.7).toInt())
                          : Colors.grey,
                    ))),
            ),
          ),
          //list
          SizedBox(
              height: _hasFocus ? _listExtenedHeight : _listHeight,
              child: AnimatedOpacity(
                opacity: _hasFocus ? 1.0 : 0.6,
                duration: const Duration(milliseconds: 100),
                child: SelectableListView(
                    key: listKey,
                    padding: EdgeInsets.symmetric(horizontal: 58),
                    itemCount: _itemCount,
                    itemBuilder: (context, index, selectedIndex, key) {
                      return Container(
                          clipBehavior: Clip.none,
                          margin: EdgeInsets.symmetric(horizontal: 10),
                          child: MediaCard.circleLarge(
                            key: key,
                            imageUrl: widget.casts[index].profileUrl,
                            isSelected: Focus.of(context).hasFocus &&
                                index == selectedIndex,
                            title: _hasFocus ? widget.casts[index].name : null,
                            subtitle:
                                _hasFocus ? widget.casts[index].character : null,
                          ));
                    }),
              )),
        ],
      ),
    );
  }
}
