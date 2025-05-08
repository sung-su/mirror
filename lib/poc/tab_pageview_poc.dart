import 'package:flutter/material.dart';
import 'package:expandable_page_view/expandable_page_view.dart';
import 'package:tizen_fs/screens/sample_pages.dart';
import 'package:tizen_fs/screens/home_page.dart';

class TvPageView extends StatefulWidget {
  const TvPageView({
    super.key,
    required this.pageController,
    required this.scrollController,
  });

  final PageController pageController;
  final ScrollController scrollController;

  @override
  State<TvPageView> createState() => _TvPageViewState();
}

class _TvPageViewState extends State<TvPageView> {

  @override
  void initState() {
    super.initState();

    widget.pageController.addListener(() {
      //TODO: to add opacity change animation when the page is changed
      debugPrint('[_TvPageViewState][pageController.addListener] widget.pageController.page=${widget.pageController.page}');
      
    });
  }

  @override
  Widget build(BuildContext context) {
    return ExpandablePageView(
      controller: widget.pageController,
      physics: const NeverScrollableScrollPhysics(),
      children: [
        AnimatedPage(
          child: HomePage(scrollController: widget.scrollController)
        ),
        AnimatedPage(
          child: ListPage(scrollController: widget.scrollController)
        ),
        AnimatedPage(
          child: EmptyPage()
        ),
      ]
    );
  }
}

class AnimatedPage extends StatelessWidget {
  const AnimatedPage({super.key, required this.child, this.isVisible = true});

  final Widget child;
  final bool isVisible;

  @override
  Widget build(BuildContext context) {
    return AnimatedOpacity(
      opacity: isVisible ? 1.0 : 0.0,
      duration: const Duration(milliseconds: 300),
      child: this.child
    );
  }
}