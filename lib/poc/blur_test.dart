import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:gradient_borders/box_borders/gradient_box_border.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class BlurTestPage extends StatefulWidget {
  const BlurTestPage({super.key});

  @override
  State<BlurTestPage> createState() => _BlurTestPageState();
}

class _BlurTestPageState extends State<BlurTestPage> {

  late List<Movie> _movies;
  bool _isEffectOn = false;
  late FocusNode _focusNode =FocusNode();

  @override
  void initState() {
    super.initState();
    _movies = context.read<MovieViewModel>().movies;
  }

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey,
      body: Stack(
        children: [
          SizedBox.expand(
              child: Image.asset('assets/mock/images/backdrop1.png', fit: BoxFit.cover),
          ),
          Focus(
            focusNode: _focusNode,
            onKeyEvent: (focusnode, event) {
              if (event is KeyDownEvent || event is KeyRepeatEvent) {
                if (event.logicalKey == LogicalKeyboardKey.enter || event.logicalKey == LogicalKeyboardKey.select) {
                  setState(() {
                    _isEffectOn = !_isEffectOn;
                  });
                  return KeyEventResult.handled;    
                }
              }
              return KeyEventResult.ignored;
            },
            child: Column(
              spacing: 10,
              children: [
                SizedBox(
                  height: 250
                ),
                CustomListView(
                  reviews: _movies[1].reviews,
                  isEffectOn: _isEffectOn,
                ),
                Container(
                  color:Colors.amber,
                  child: Text(
                    "* To turn on/off the blur effect, press Enter(Select) key *",
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.black)
                  )
                )
              ],
            ),
          ),
        ],
      ),
    );
  }
}



class CustomListView extends StatefulWidget {
  final VoidCallback? onFocused;
  final VoidCallback? onSelectionChanged;
  final List<Reviews> reviews;
  final bool isEffectOn;

  const CustomListView({
    super.key,
    required this.reviews,
    required this.isEffectOn,
    this.onFocused,
    this.onSelectionChanged
  });

  @override
  State<CustomListView> createState() => _CustomListViewState();
}

class _CustomListViewState extends State<CustomListView> with FocusSelectable<CustomListView>  {

  bool _hasFocus = false;
  int _itemCount = 0;
  double _listHeight = 110;

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
          onSelectionChanged: () => widget.onSelectionChanged?.call(),
          itemBuilder: (context, index, selectedIndex, key) {
            final review = widget.reviews[index];
            return Container(
                clipBehavior: Clip.none,
                margin: EdgeInsets.symmetric(horizontal: 15),
                child: CardView(
                  key: key,
                  title: review.author,
                  description: review.content,
                  isSelected: Focus.of(context).hasFocus &&
                      index == selectedIndex,
                  isEffectOn: widget.isEffectOn
                )
            );
          })
      ),
    );
  }
}



class CardView extends StatelessWidget {
  const CardView(
      {super.key,
      this.width = 390,
      this.title,
      this.description,
      this.isSelected = false,
      this.shadowColor,
      this.isEffectOn = false})
      : height = 110;

  static const int animationDuration = 200;
  final double width;
  final double height;
  final String? title;
  final String? description;
  final bool isSelected;
  final Color? shadowColor;
  final bool isEffectOn;

  @override
  Widget build(BuildContext context) {
    return Column(
      spacing: 1,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        AnimatedScale(
          scale: isSelected ? 1.1 : 1,
          duration: Duration(milliseconds: animationDuration),
          child: _buildBorder(
            _buildTileContent()
          ),
        )
      ],
    );
  }

  Widget _buildBorder(Widget content) {
    // // TODO: performance issue when using blur effect
    return isEffectOn ?
    ClipRRect(
      borderRadius: BorderRadius.circular(16),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 5, sigmaY: 5),
        child: Container(
          width: width,
          height: height,
          padding : EdgeInsets.all(1),
          decoration: BoxDecoration(
            border: const GradientBoxBorder(
              gradient: LinearGradient(colors: [Colors.blue, Colors.pink]),
              width: 1,
            ),
            borderRadius: BorderRadius.circular(16),
            color: Colors.black.withAlphaF(0.5)
          ),
          child: content
        ),
      ),
    )
    : Container(
      width: width,
      height: height,
      padding : EdgeInsets.all(1),
      decoration: BoxDecoration(
        border: GradientBoxBorder(
          gradient: LinearGradient(colors: [Colors.blue, Colors.pink]),
          width: isSelected ? 1.5 : 1,
        ),
        borderRadius: BorderRadius.circular(16),
        color: Colors.black.withAlphaF(0.5)
      ),
      child: content
    );
  }

  Widget _buildTileContent() {
      return Padding(
        padding: const EdgeInsets.only(left: 20, right: 20, top: 20, bottom: 20),
        child: Column(
          spacing: 10,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              spacing: 10,
              children: [
                Icon(
                  Icons.people_outline,
                  size: 17,
                ),
                Text(this.title!, style: const TextStyle(fontSize: 15)),
                Spacer(),
              ],
            ),
            Text(
              this.description!,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: const TextStyle(fontSize: 12)
            ),
          ],
        ),
      );
  }
}