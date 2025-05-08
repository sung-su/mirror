import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';
import 'package:tizen_fs/widgets/tab_bar.dart';
import 'package:tizen_fs/widgets/tab_pageview.dart';

class HomePocPage extends StatelessWidget {
  const HomePocPage({super.key});

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
  final PageModel _pageModel = PageModel(['Home', 'Apps', 'Library']);

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (_) => _pageModel,
      child: CustomScrollView(
        controller: _scrollController,
        slivers: [
          SliverAppBar(
            pinned: false,
            floating: false,
            automaticallyImplyLeading: false,
            title: TvTabbar(),
          ),
          SliverToBoxAdapter(
            child: Consumer<PageModel>(builder: (context, model, child) {
              return TvPageView(
                scrollController: _scrollController,
              );
            },)
          )
        ],
      )
    );
  }
}