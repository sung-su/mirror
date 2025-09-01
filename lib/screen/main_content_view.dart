import 'dart:math';
import 'package:flutter/material.dart';
import 'package:expandable_page_view/expandable_page_view.dart';
import 'package:flutter/rendering.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/screen/home_page.dart';
import 'package:tizen_fs/screen/media_page.dart';
import 'package:tizen_fs/styles/app_style.dart';

class MainContentView extends StatefulWidget {
  const MainContentView({
    super.key,
    required this.pageController,
    required this.scrollController,
    required this.register,
    required this.unregister,
  });

  final PageController pageController;
  final ScrollController scrollController;
  final void Function(int, Function(ScrollDirection, bool)) register;
  final void Function(int) unregister;

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
        _buildFadingPage(index: 0, child: HomePage(
          scrollController: widget.scrollController, 
          register: widget.register,
          unregister: widget.unregister,)),
        _buildFadingPage(index: 1, child: MediaPage()),
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