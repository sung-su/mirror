import 'dart:math';
import 'package:flutter/material.dart';
import 'package:expandable_page_view/expandable_page_view.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/screens/mock_apps_page.dart';
import 'package:tizen_fs/screens/mock_library_page.dart';
import 'package:tizen_fs/utils/media_db_parser.dart';
import 'package:tizen_fs/screens/search_page.dart';
import 'home_page.dart';

class HomeContentView extends StatefulWidget {
  const HomeContentView({
    super.key,
    required this.pageController,
    required this.scrollController,
  });

  final PageController pageController;
  final ScrollController scrollController;

  @override
  State<HomeContentView> createState() => _HomeContentViewState();
}

class _HomeContentViewState extends State<HomeContentView> {
  late final PageController _controller;
  double _currentPage = 0;

  @override
  void initState() {
    super.initState();
    _controller = widget.pageController;
    _controller.addListener(() {
      setState(() {
        _currentPage = _controller.page ?? _controller.initialPage.toDouble();
      });
    });
  }

  @override
  Widget build(BuildContext context) {
    final categories = Provider.of<MediaDBParser>(context).categories;
    return categories.isEmpty
        ? SizedBox(
            height: max(200, MediaQuery.of(context).size.height - 200),
            child: const Center(child: CircularProgressIndicator()))
        : ExpandablePageView(
            controller: _controller,
            physics: const NeverScrollableScrollPhysics(),
            children: [
              _buildFadingPage(
                  index: 0,
                  child: HomePage(
                      scrollController: widget.scrollController,
                      categories: categories)),
              _buildFadingPage(index: 1, child: MockAppsPage()),
              _buildFadingPage(index: 2, child: MockLibraryPage()),
              _buildFadingPage(
                  index: 3,
                  child:
                      SearchPage(scrollController: widget.scrollController))
            ]);
  }

  Widget _buildFadingPage({required int index, required Widget child}) {
    double opacity = 1.0 - min((_currentPage - index).abs(), 1.0);

    return AnimatedOpacity(
      duration: const Duration(milliseconds: 500),
      opacity: opacity,
      child: child,
    );
  }
}
