import 'dart:math';
import 'package:flutter/material.dart';
import 'package:expandable_page_view/expandable_page_view.dart';
import 'package:tizen_fs/screen/home_page.dart';
import 'package:tizen_fs/screen/mock_apps_page.dart';

class MainContentView extends StatefulWidget {
  const MainContentView({
    super.key,
    required this.pageController,
    required this.scrollController,
  });

  final PageController pageController;
  final ScrollController scrollController;

  @override
  State<MainContentView> createState() => _MainContentViewState();
}

class _MainContentViewState extends State<MainContentView> {
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
    return ExpandablePageView(
      controller: _controller,
      animationDuration: const Duration(milliseconds: 50),
      physics: const NeverScrollableScrollPhysics(),
      children: [
        _buildFadingPage(index: 0, child: HomePage()),
        _buildFadingPage(index: 1, child: MockAppsPage(isHorizontal:  true)),
        _buildFadingPage(index: 2, child: MockAppsPage(isHorizontal: false)),
      ]);
  }

  Widget _buildFadingPage({required int index, required Widget child}) {
    double opacity = 1.0 - min((_currentPage - index).abs(), 1.0);

    return AnimatedOpacity(
      duration: const Duration(milliseconds: 100),
      opacity: opacity,
      child: child,
    );
  }
}


