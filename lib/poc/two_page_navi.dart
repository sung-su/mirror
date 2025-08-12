import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/screen/mock_apps_page.dart';
import 'package:tizen_fs/styles/app_style.dart';

class TwoPageNavigation extends StatefulWidget {
  final String title;
  final Widget? masterPage;
  final Widget? detailPage;
  final Function(int)? onFocusedItemChanged;
  final Function(int)? onPageChanged;

  TwoPageNavigation({
    super.key,
    this.title = "",
    this.masterPage,
    this.detailPage,
    this.onFocusedItemChanged,
    this.onPageChanged,
  });

  @override
  State<TwoPageNavigation> createState() => TwoPageNavigationState();
}

class TwoPageNavigationState extends State<TwoPageNavigation> {
  final FocusNode _focusNode = FocusNode();
  final PageController _pageController = PageController(
    viewportFraction: 0.6
  );

  List<String> _pages = ["Settings", ""];
  List<Color> _colors = [Colors.red, Colors.green, Colors.blue];

  List<GlobalKey> _itemKeys = List.generate(3, (_) => GlobalKey());

  int _current = 0;

  void move() {
    _pageController.animateToPage(
      _current,
      duration: $style.times.fast,
      curve: Curves.easeInOut
    );
  }

  int focusedItemIndex = 0;
  int current = 0;
  int pageIndex = 0;

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        setState(() {
          _current = (_current < _pages.length - 2) ? _current + 1 : _current;  
        });
        move();
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        setState(() {
          _current = (_current > 0) ? _current - 1 : _current;
        });
        move();
        return KeyEventResult.handled;
      } 
    }
    return KeyEventResult.ignored;
  }

  void _selectTo(int index)
  {
    if(index == _pages.length -1 ) return;

    setState(() {
      _current = index;
    });
    move();
  }

  void _updatePages(int selected){

    var current = _current;
    List newItems = [];

    if (selected == 0) {
      newItems = ["Accounts", ""];
    }
    else if ( selected == 1) {
      newItems = ["Wi-Fi", ""];
    }
    else if (selected == 2) {
      newItems = ["About Device", ""];
    }

    final List newKeys = List.generate(2, (_) => GlobalKey());
    
    setState(() {
      _itemKeys = [..._itemKeys.sublist(0, current + 1), ...newKeys];
      _pages = [..._pages.sublist(0, current + 1), ...newItems];
    });
  }

  @override
  void initState() {
    super.initState();
    focusedItemIndex = 0;
    current = 0;
    pageIndex = 0;
  }

  @override
  void didUpdateWidget(covariant TwoPageNavigation oldWidget) {
    super.didUpdateWidget(oldWidget);
  }

  @override
  void dispose() {
    _pageController.dispose();
    _focusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body:Focus(
        focusNode: _focusNode,
        onKeyEvent: _onKeyEvent,
        child: PageView.builder(
            controller: _pageController,
            padEnds: false,
            scrollDirection: Axis.horizontal,
            itemCount: _pages.length,
            physics: NeverScrollableScrollPhysics(),
            itemBuilder: (context, index) {
              return GestureDetector(
                onTap: () {
                  _selectTo(index);
                },
                child: ColoredPage(
                  key: _itemKeys[index],
                  title: _pages[index],
                  isEnabled: index == _current,
                  backgroundcColor: _colors[index % 3],
                  onSelectionChanged: (selected) {
                    _updatePages(selected);
                  },
                ),
              );
            },
        )
      )
    );
  }
}

class ColoredPage extends StatefulWidget {
  const ColoredPage({super.key, required this.title, required this.backgroundcColor, required this.onSelectionChanged, required this.isEnabled});

  final Function(int)? onSelectionChanged;

  final Color backgroundcColor;
  final String title;
  final bool isEnabled;

  @override
  State<ColoredPage> createState() => _ColoredPageState();
}

class _ColoredPageState extends State<ColoredPage> {
  GlobalKey<MockListState> _listKey = GlobalKey<MockListState>();

  @override
  void initState() {
    super.initState();
    if (widget.isEnabled) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _listKey.currentState?.initFocus();
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      color: widget.isEnabled ? widget.backgroundcColor : Colors.grey.shade600,
      child: Column(
        children: [
          SizedBox(
            height: 120,
            child: Padding(
              padding: const EdgeInsets.fromLTRB(80, 0, 80, 0),
              child: Align(
                alignment: Alignment.bottomLeft,
                child: Container(
                  // color: Colors.amber,
                  child: Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 15),
                    child: Text(
                      widget.title,
                      style: TextStyle(
                        fontSize: 30
                      ),
                    ),
                  )
                ),
              ),
            )
          ),
          Expanded(
            child: Align(
              alignment: Alignment.topLeft,
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 80),
                child: Container(
                  height: double.infinity,
                  width: double.infinity,
                  // color: Colors.grey,
                  child: 
                  widget.title != null && widget.title.isNotEmpty ? 
                  Column(
                    mainAxisAlignment: MainAxisAlignment.start,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ElevatedButton(
                        onPressed: () {
                          widget.onSelectionChanged?.call(0);
                        }, 
                        child: Text("Account"),
                      ),
                      ElevatedButton(
                        onPressed: () {
                          widget.onSelectionChanged?.call(1);
                        }, 
                        child: Text("Wi-Fi"),
                      ),
                      ElevatedButton(
                        onPressed: () {
                          widget.onSelectionChanged?.call(2);
                        }, 
                        child: Text("About Device"),
                      )
                    ],
                  ) : SizedBox.shrink(),
                ),
              ),
            )
          )
        ],
      ),
    );
  }
}
