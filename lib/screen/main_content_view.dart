import 'dart:math';
import 'package:flutter/material.dart';
import 'package:expandable_page_view/expandable_page_view.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'package:tizen_fs/screen/home_page.dart';
import 'package:tizen_fs/screen/mock_apps_page.dart';
import 'package:tizen_fs/styles/app_style.dart';

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
      physics: const NeverScrollableScrollPhysics(),
      children: [
        _buildFadingPage(index: 0, child: HomePage(scrollController: widget.scrollController)),
        _buildFadingPage(index: 1, child: Mockpage()),
        // _buildFadingPage(index: 2, child: MockAppsPage(isHorizontal: true)),
      ]);
  }

  Widget _buildFadingPage({required int index, required Widget child}) {
    double opacity = 1.0 - min((_currentPage - index).abs(), 1.0);

    return AnimatedOpacity(
      duration: $style.times.fast,
      opacity: opacity,
      child: child,
    );
  }
}

class Mockpage extends StatefulWidget{
  @override
  State<Mockpage> createState() => _MockpageState();
}

class _MockpageState extends State<Mockpage> {

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(null);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container();
  }
}
