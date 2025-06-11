import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class ImmersiveContent {
  final String title;
  final String description;
  final String subtitle;
  final String backdrop;
  final String card;

  ImmersiveContent({
    required this.title,
    required this.description,
    required this.subtitle,
    required this.backdrop,
    required this.card,
  });

  factory ImmersiveContent.fromJson(Map<String, dynamic> json) {
    return ImmersiveContent(
      title: json['title'] as String,
      description: (json['description'] as String).length > 100
          ? '${json['description'].substring(0, 100)}...'
          : json['description'] as String,
      subtitle: json['metadata'] as String,
      backdrop: json['backdrop'] as String,
      card: json['card'] as String,
    );
  }

  static Future<List<ImmersiveContent>> loadFromJson() async {
    final jsonString =
        await rootBundle.loadString('assets/mock/mock_content.json');
    final List<dynamic> jsonList = jsonDecode(jsonString);
    final List<ImmersiveContent> contents =
        jsonList.map((json) => ImmersiveContent.fromJson(json)).toList();

    return contents;
  }

  @override
  String toString() {
    return 'MediaContent(title: $title, description: $description, subtitle: $subtitle, backdrop: $backdrop, card: $card)';
  }

  static List<ImmersiveContent> generateMockContent() {
    return List.generate(
      20,
      (index) => ImmersiveContent(
        title: 'Power Sisters $index',
        description:
            'A dynmaic duo of superhero siblings join forces to save their city from a sinister vailain, redefining sisterhood in action. $index',
        subtitle: 'Subtitle $index',
        backdrop: 'backdrop${(index % 3) + 1}.png',
        card: 'ContentPath $index',
      ),
    );
  }
}

class ImmersiveListModel extends ChangeNotifier {
  late List<ImmersiveContent> contents;
  bool _isLoading = false;
  int _selectedIndex = 0;

  ImmersiveListModel(this.contents);

  ImmersiveListModel.fromMock() {
    _isLoading = true;
    ImmersiveContent.loadFromJson().then((value) {
      _isLoading = false;
      contents = value;
      notifyListeners();
    });
  }

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    _selectedIndex = index;
    notifyListeners();
  }

  int get itemCount => _isLoading ? 0 : contents.length;
  ImmersiveContent getContent(int index) {
    if (_isLoading) {
      return ImmersiveContent(
        title: 'Loading...',
        description: 'Loading...',
        subtitle: 'Loading...',
        backdrop: 'Loading...',
        card: 'Loading...',
      );
    }
    return contents[index];
  }

  ImmersiveContent getSelectedContent() {
    if (_isLoading) {
      return ImmersiveContent(
        title: 'Loading...',
        description: 'Loading...',
        subtitle: 'Loading...',
        backdrop: 'Loading...',
        card: 'Loading...',
      );
    }
    return contents[_selectedIndex];
  }
}

class ImmersiveContentArea extends StatelessWidget {
  const ImmersiveContentArea({
    super.key,
  });
  static const double leftPadding = 58;
  @override
  Widget build(BuildContext context) {
    return Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
      Center(
          child: Padding(
              padding: EdgeInsets.only(top: 10),
              child: Icon(Icons.keyboard_arrow_up,
                  size: 35,
                  color: Theme.of(context).indicatorColor.withAlphaF(0.5)))),
      Padding(
          padding: EdgeInsets.only(left: leftPadding, top: 5, bottom: 0),
          child: Text(
            'Top picks for you',
            textAlign: TextAlign.left,
            style: TextStyle(
                fontSize: 24,
                color: Theme.of(context).colorScheme.onSurface.withAlphaF(0.5)),
          )),
      Expanded(child: SizedBox.shrink()),
      Padding(
        padding: EdgeInsets.only(left: leftPadding),
        child: AnimatedSwitcher(
          duration: const Duration(milliseconds: 150),
          transitionBuilder: (Widget child, Animation<double> animation) {
            return FadeTransition(
              opacity: animation,
              child: child,
            );
          },
          child: SizedBox(
              key: ValueKey(
                  Provider.of<ImmersiveListModel>(context).selectedIndex),
              width: 480,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  Text(
                    Provider.of<ImmersiveListModel>(context)
                        .getSelectedContent()
                        .title,
                    style: TextStyle(
                      fontSize: 40,
                      color: Colors.white,
                    ),
                    textAlign: TextAlign.left,
                  ),
                  SizedBox(height: 8),
                  Text(
                    Provider.of<ImmersiveListModel>(context)
                        .getSelectedContent()
                        .subtitle,
                    style: TextStyle(fontSize: 18, color: Colors.grey),
                  ),
                  SizedBox(height: 5),
                  Text(
                    Provider.of<ImmersiveListModel>(context)
                        .getSelectedContent()
                        .description,
                    style: TextStyle(fontSize: 16, color: Colors.grey),
                    softWrap: true,
                  ),
                ],
              )),
        ),
      )
    ]);
  }
}

class ImmersiveListArea extends StatefulWidget {
  final VoidCallback? onFocused;
  final void Function(int index)? onExecute;

  const ImmersiveListArea(
      {super.key, this.onFocused, this.onExecute});

  @override
  State<ImmersiveListArea> createState() => _ImmersiveListAreaState();
}

class _ImmersiveListAreaState extends State<ImmersiveListArea> {
  final FocusNode _focusNode = FocusNode();
  final GlobalKey<SelectableListViewState> _listViewKey =
      GlobalKey<SelectableListViewState>();

  List<ImmersiveContent> contents = [];

  bool _hasFocus = false;
  static const double _leftPadding = 58;
  int _itemCount = 0;
  int _selectedIndex = 0;

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_onFocusChanged);
    if (Provider.of<ImmersiveListModel>(context, listen: false).itemCount ==
        0) {
      Provider.of<ImmersiveListModel>(context, listen: false)
          .addListener(_handleModelUpdate);
    } else {
      contents =
          Provider.of<ImmersiveListModel>(context, listen: false).contents;
      _itemCount = contents.length;
    }
  }

  void _handleModelUpdate() {
    setState(() {
      contents =
          Provider.of<ImmersiveListModel>(context, listen: false).contents;
      _itemCount = contents.length;
    });
    Provider.of<ImmersiveListModel>(context, listen: false)
        .removeListener(_handleModelUpdate);
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
      Provider.of<BackdropProvider>(context, listen: false)
          .updateBackdrop(getSelectedBackdrop());
    } else {
      Provider.of<BackdropProvider>(context, listen: false)
          .updateBackdrop(null);
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
        widget.onExecute?.call(_selectedIndex);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  Future<void> _next(bool fast) async {
    if (_selectedIndex >= _itemCount - 1) {
      return;
    }
    Provider.of<ImmersiveListModel>(context, listen: false).selectedIndex = ++_selectedIndex;
    var moved = await _listViewKey.currentState?.next(fast: fast);
    _selectedIndex = moved ?? _selectedIndex;
    final current = _selectedIndex;

    await Future.delayed(const Duration(milliseconds: 300));

    if (current == _selectedIndex) {
      Provider.of<BackdropProvider>(context, listen: false)
          .updateBackdrop(getSelectedBackdrop());
    }
  }

  Future<void> _prev(bool fast) async {
    if (_selectedIndex <= 0) {
      return;
    }
    Provider.of<ImmersiveListModel>(context, listen: false).selectedIndex = --_selectedIndex;
    var moved = await _listViewKey.currentState?.previous(fast: fast);
    _selectedIndex = moved ?? _selectedIndex;
    final current = _selectedIndex;

    await Future.delayed(const Duration(milliseconds: 300));

    if (current == _selectedIndex) {
      Provider.of<BackdropProvider>(context, listen: false)
          .updateBackdrop(getSelectedBackdrop());
    }
  }

  Widget getSelectedBackdrop() {
    return Container(
        key: ValueKey(_selectedIndex),
        alignment: Alignment.topRight,
        child: SizedBox(
          width: 758,
          height: 426,
          child: CinematicScrim(
            image: Image.asset(
                'assets/mock/images/${Provider.of<ImmersiveListModel>(context, listen: false).getSelectedContent().backdrop}',
                width: 758,
                height: 426,
                fit: BoxFit.fill),
          ),
        ));
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _onKeyEvent,
      child: Column(
        children: [
          AnimatedOpacity(
            duration: const Duration(milliseconds: 100),
            opacity: _hasFocus ? 0.0 : 1.0,
            child: Container(
              alignment: Alignment.topLeft,
              padding: EdgeInsets.only(left: _leftPadding, top: 10, bottom: 10),
              child: _hasFocus
                  ? SizedBox(
                      height: 5,
                    )
                  : const Text('Top picks for you',
                      textAlign: TextAlign.left,
                      style: TextStyle(
                        fontSize: 16,
                        color: Colors.grey,
                      )),
            ),
          ),
          SizedBox(
              height: 115,
              child: SelectableListView(
                  key: _listViewKey,
                  padding:
                      EdgeInsets.only(left: _leftPadding, right: _leftPadding),
                  itemCount: _itemCount,
                  itemBuilder: (context, index, selectedIndex, key) {
                    return Container(
                        clipBehavior: Clip.none,
                        margin: EdgeInsets.symmetric(horizontal: 10),
                        child: MediaCard.fiveCard(
                            key: key,
                            imageUrl:
                                'assets/mock/images/${contents[index].card}',
                            isSelected: Focus.of(context).hasFocus &&
                                index == selectedIndex));
                  })),
        ],
      ),
    );
  }
}

class CinematicScrim extends StatelessWidget {
  const CinematicScrim({super.key, required this.image});

  final Widget image;

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        Padding(
          padding: const EdgeInsets.all(2.0),
          child: image,
        ),
        Container(
          decoration: BoxDecoration(
            gradient: RadialGradient(
              center: Alignment(0.8, -0.8),
              radius: 2,
              colors: [
                const Color.fromARGB(255, 18, 18, 18)
                    .withAlpha((0.1 * 255).toInt()),
                const Color.fromARGB(255, 18, 18, 18),
              ],
              stops: const [0, 0.7],
            ),
          ),
        ),
      ],
    );
  }
}
