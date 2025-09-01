import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/app_data_model.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';
import 'main_top_menu.dart';
import 'main_content_view.dart';

class MainScreen extends StatelessWidget {
  const MainScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return BackdropScaffold(child: const MainContent());
  }
}

class MainContent extends StatefulWidget {
  const MainContent({super.key});

  @override
  State<MainContent> createState() => _MainContentState();
}

class _MainContentState extends State<MainContent> {
  final ScrollController _scrollController = ScrollController(keepScrollOffset: true);
  final PageController _pageController = PageController(initialPage: 0);


  final Map<int, Function(ScrollDirection, bool)> _childCallbacks = {};
  bool _userScrolling = false;
  ScrollDirection _scrollDirection = ScrollDirection.idle;

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    _pageController.dispose();
    super.dispose();
  }
    
  @override
   Widget build(BuildContext context) {
    return NotificationListener<ScrollNotification>(
      onNotification: (notification) {
        if (notification is UserScrollNotification) {
          if (notification.direction != ScrollDirection.idle) {
            _userScrolling = true;
            _scrollDirection = notification.direction;
          }
        }
    
        if (notification is ScrollUpdateNotification) {
          if (_userScrolling) {
            _notifyChildren(_scrollDirection, false);
          }
        }
    
        if (notification is ScrollEndNotification) {
          if (_userScrolling) {
            _notifyChildren(_scrollDirection, true);
          }
          _userScrolling = false;
        }
    
        return false;
      },
      child: CustomScrollView(
        scrollBehavior: ScrollBehavior().copyWith(
          scrollbars: false,
          overscroll: false,
          dragDevices: {
            PointerDeviceKind.mouse,
            PointerDeviceKind.touch
          }
        ),
        controller: _scrollController,
        primary: false,
        slivers: [
          SliverAppBar(
            pinned: false,
            floating: false,
            automaticallyImplyLeading: false,
            toolbarHeight: 80,
            backgroundColor: Colors.transparent,
            title: MainTopMenu(
              pageController: _pageController,
            ),
          ),
          SliverToBoxAdapter(
            child: MainContentView(
              pageController: _pageController,
              scrollController: _scrollController,
              register: registerChild,
              unregister: unregisterChild,
            )
          )
        ],
      ),
    );
  }

  void registerChild(int index, Function(ScrollDirection, bool) callback) {
    _childCallbacks[index] = callback;
  }

  void unregisterChild(int index) {
    _childCallbacks.remove(index);
  }

  void _notifyChildren(ScrollDirection direction, bool scrollEnd) {
    for (var callback in _childCallbacks.values) {
      callback(direction, scrollEnd);
    }
  }

}