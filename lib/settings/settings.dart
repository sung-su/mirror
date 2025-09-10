import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/locator.dart';
import 'package:tizen_fs/models/bt_model.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/models/settings_menus.dart';
import 'package:tizen_fs/settings/setting_page_interface.dart';
import 'package:tizen_fs/settings/setting_page.dart';
import 'package:tizen_fs/styles/app_style.dart';

class Settings extends StatefulWidget {
  const Settings({super.key});

  @override
  State<Settings> createState() => SettingsState();
}

class SettingsState extends State<Settings> {
  final FocusNode _focusNode = FocusNode();
  late final PageController _pageController;
  PageNode _pageTree = SettingPages().getRoot();

  late List<PageNode?> _pages;
  List<GlobalKey> _itemKeys = List.generate(3, (_) => GlobalKey());

  int _current = 0;
  double viewportFraction = 0.585;

  void move() {
    _pageController.animateToPage(
      _current,
      duration: $style.times.med,
      curve: Curves.easeInOut,
    );
  }

  @override
  void initState() {
    super.initState();
    _pageController = PageController(
      viewportFraction: viewportFraction,
      keepPage: false,
    );
    _pages = [_pageTree, null];

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setupBtModel();
    });
  }

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowRight ||
          event.logicalKey == LogicalKeyboardKey.enter) {
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
      } else if (event.logicalKey == LogicalKeyboardKey.goBack ||
          event.logicalKey == LogicalKeyboardKey.escape ||
          event.physicalKey == PhysicalKeyboardKey.escape) {
        if (_current == 0) Navigator.pop(context);
        setState(() {
          _current = (_current > 0) ? _current - 1 : _current;
        });
        move();
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  void _selectTo(int index) {
    if (index == _pages.length - 1) return;
    setState(() {
      _current = index;
    });
    move();
  }

  void _updatePages(PageNode? node, int selected) {
    if (node == null) return;

    final state = _itemKeys[_current + 1]?.currentState;
    if (state is SettingPageInterface) {
      (state as SettingPageInterface)?.hidePage();
    }

    var current = _current;
    List newItems = [];

    newItems = [node.children[selected]];
    final List newKeys = List.generate(1, (_) => GlobalKey());

    if (!node.children[selected].isEnd) {
      newItems.add(null);
      newKeys.add(GlobalKey());
    }

    setState(() {
      _itemKeys = [..._itemKeys.sublist(0, current + 1), ...newKeys];
      _pages = [..._pages.sublist(0, current + 1), ...newItems];
    });
  }

  @override
  void didUpdateWidget(covariant Settings oldWidget) {
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
      body: Focus(
        focusNode: _focusNode,
        onKeyEvent: _onKeyEvent,
        child: PageView.builder(
          controller: _pageController,
          padEnds: false,
          scrollDirection: Axis.horizontal,
          itemCount: _pages.length,
          physics: NeverScrollableScrollPhysics(),
          itemBuilder: (context, index) {
            if (_pages[index] == null) {
              return Container(color: Theme.of(context).colorScheme.onTertiary);
            } else {
              return GestureDetector(
                onTap: () {
                  _selectTo(index);
                },
                child: SettingPage(
                  key: _itemKeys[index],
                  node: _pages[index]!,
                  isEnabled: index <= _current,
                  onItemFocused: (focused) {
                    _updatePages(_pages[index], focused);
                  },
                  onItemSelected: (selected) {
                    _updatePages(_pages[index], selected);
                    WidgetsBinding.instance.addPostFrameCallback((_) {
                      _selectTo(index + 1);
                    });
                  },
                ),
              );
            }
          },
        ),
      ),
    );
  }
}
