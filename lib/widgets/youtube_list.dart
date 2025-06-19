import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/review_list.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class YoutubeList extends StatefulWidget {
  final VoidCallback? onFocused;
  final List<Video> videos;
  final String title;

  const YoutubeList({
    super.key,
    required this.videos,
    this.onFocused,
    this.title = '',
  });

  @override
  State<YoutubeList> createState() => _YoutubeListState();
}

class _YoutubeListState extends State<YoutubeList> with FocusSelectable<YoutubeList> {
  late String _title;

  bool _hasFocus = false;
  late final int _itemCount;
  final double _titleFontSize = 14;
  final double _listHeight = 170;
  final double _listExtendedHeight = 250;

  @override
  void initState() {
    super.initState();
    focusNode.addListener(_onFocusChanged);
    _itemCount = widget.videos.length; // 104
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
        spacing: 10,
        children: [
          //list title
          Padding(
            padding: const EdgeInsets.fromLTRB(70, 0, 70, 8),
            child: SizedBox(
              height: _hasFocus ? 40 : 20,
              child: AnimatedScale(
                  scale: _hasFocus ? 1.7 : 1.0,
                  duration: const Duration(milliseconds: 100),
                  alignment: Alignment.topLeft,
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    spacing: 5,
                    children: [
                      if (_title.isNotEmpty)
                        Text(_title,
                            textAlign: TextAlign.left,
                            style: TextStyle(
                              fontSize: _titleFontSize,
                              color: _hasFocus
                                  ? Colors.white
                                      .withAlpha((255 * 0.7).toInt())
                                  : Colors.grey,
                            )),
                    ],
                  )),
            ),
          ),
          //list
          SizedBox(
              height: _hasFocus ? _listExtendedHeight : _listHeight,
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
                          child: MediaCard.threeCard(
                            key: key,
                            imageUrl:
                                widget.videos[index].youtubeThumbnail,
                            isSelected: Focus.of(context).hasFocus &&
                                index == selectedIndex,
                            title: _hasFocus ? widget.videos[index].name : null,
                            subtitle: _hasFocus
                                ? widget.videos[index].publishedYear
                                : null,
                            description:
                                _hasFocus ? widget.videos[index].type : null,
                          ));
                    }),
              )),
        ],
      ),
    );
  }
}
