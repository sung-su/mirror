import 'package:flutter/material.dart';
import 'package:expandable_page_view/expandable_page_view.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/screens/mock_library_page.dart';
import 'package:tizen_fs/utils/media_db_parser.dart';
import 'sample_pages.dart';
import 'home_page.dart';

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
  Widget build(BuildContext context) {
    return ExpandablePageView(
        controller: widget.pageController,
        physics: const NeverScrollableScrollPhysics(),
        children: [
          HomePage(
              scrollController: widget.scrollController,
              categories: Provider.of<MediaDBParser>(context, listen: false)
                  .categoryMap
                  .values
                  .toList()),
          ListPage(scrollController: widget.scrollController),
          MockLibraryPage()
        ]);
  }
}
