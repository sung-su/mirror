import 'package:flutter/material.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';
import 'tab_bar.dart';
import 'tab_pageview.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return BackdropScaffold(child: const HomeContent());
  }
}

class HomeContent extends StatefulWidget {
  const HomeContent({super.key});

  @override
  State<HomeContent> createState() => _HomeContentState();
}

class _HomeContentState extends State<HomeContent> {
  final ScrollController _scrollController = ScrollController();
  final PageController _pageController = PageController(initialPage: 0);
  final FocusNode _headerFocusNode = FocusNode();

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
    return CustomScrollView(
      controller: _scrollController,
      slivers: [
        SliverAppBar(
          pinned: false,
          floating: false,
          automaticallyImplyLeading: false,
          toolbarHeight: 80,
          backgroundColor: Colors.transparent,
          title: TvTabbar(
            pageController: _pageController,
            focusNode: _headerFocusNode,
          ),
        ),
        SliverToBoxAdapter(
          child: TvPageView(
              pageController: _pageController,
              scrollController: _scrollController,
              headerFocusNode: _headerFocusNode,
          )
        )
      ],
    );
  }
}