import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/app_list.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';
import 'home_top_menu.dart';
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
  final AppInfoModel _appInfoModel = AppInfoModel.fromMock(20);

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
    return ChangeNotifierProvider(
      create: (context) => _appInfoModel,
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
            title: HomeTopMenu(
              pageController: _pageController,
            ),
          ),
          SliverToBoxAdapter(
            child: MainContentView(
              pageController: _pageController,
              scrollController: _scrollController,
            )
          )
        ],
      ),
    );
  }
}