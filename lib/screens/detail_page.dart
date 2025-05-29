import 'dart:math';
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/category.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/screens/detail_footer.dart';
import 'package:tizen_fs/screens/flexible_title_detail.dart';
import 'package:tizen_fs/screens/video_backdrop.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/age_rating.dart';
import 'package:tizen_fs/screens/button_list.dart';
import 'package:tizen_fs/widgets/cast_list.dart';
import 'package:tizen_fs/screens/ifyoulike_list.dart';
import 'package:tizen_fs/screens/review_list.dart';
import 'package:tizen_fs/screens/rotten_rating.dart';
import 'package:tizen_fs/widgets/media_card.dart';
import 'package:tizen_fs/widgets/movie_list.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';
import 'package:tizen_fs/widgets/star_rating.dart';

class DetailPage extends StatefulWidget {
  final Movie movie;
  final List<Category> categories;
  const DetailPage({super.key, required this.movie, required this.categories});

  @override
  State<DetailPage> createState() => _DetailPageState();
}

class _DetailPageState extends State<DetailPage> {
  final ScrollController _scrollController = ScrollController(initialScrollOffset: 240);
  final GlobalKey<ButtonListState> _scrollAnchor = GlobalKey<ButtonListState>();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final RenderBox renderBox = _scrollAnchor.currentContext!.findRenderObject() as RenderBox;
      _scrollAnchor.currentState?.requestFocus(); // request focus on first element of the page
    });
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final Movie movie = widget.movie;
    return Scaffold(
        body: Stack(
      children: [
        Positioned.fill(
          child: VideoBackdrop(
            scrollController: _scrollController,
            imageUrl: 'https://media.themoviedb.org/t/p/w1066_and_h600_bestv2/${movie.backdropPath}',
            videoUrl: 'assets/mock/videos/conclave_trailer.mp4'
          )
        ),
        CustomScrollView(
          scrollBehavior: ScrollBehavior().copyWith(scrollbars: false, overscroll: false),
          controller: _scrollController,
          slivers: <Widget>[
            SliverAppBar(
              backgroundColor: Colors.transparent,
              automaticallyImplyLeading: false,
              toolbarHeight: 250,
              expandedHeight: MediaQuery.of(context).size.height - 50,
              flexibleSpace: Padding(
                padding: const EdgeInsets.only(left: 58),
                child: FlexibleTitleForDetail(
                  expandedHeight: MediaQuery.of(context).size.height - 50,
                  collapsedHeight: 250,
                  movie: movie,
                  onFocused: (context) {
                  }
                )
              ),
            ),
           SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.only(top: 20),
                child: Column(
                  spacing: 20,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    if (movie.reviews.isNotEmpty)
                      MockReviewList(
                        reviews: movie.reviews,
                        onFocused: (context) {
                          _scrollController.animateTo(
                            230, 
                            duration: Duration(milliseconds: 100),
                            curve: Curves.easeInQuad
                          );
                        }
                      ),
                    ButtonList(
                      key: _scrollAnchor,
                      onFocused: (context) {
                        _scrollController.animateTo(
                          230, 
                          duration: Duration(milliseconds: 100),
                          curve: Curves.easeInQuad
                        );
                      }
                    ),
                    CastList(
                      title: 'Cast & Crew',
                      cast: movie.cast,
                      onFocused: () {
                        // Scrollable.ensureVisible(
                        //   context,
                        //   alignment: 0.15,
                        //   duration: Duration(milliseconds: 100),
                        //   curve: Curves.easeInQuad
                        // );
                      }
                    ),
                    // MovieList(
                    //   title: widget.categories[5].name,
                    //   tiles: widget.categories[5].tiles,
                    //   columns: ColumnCount.six,
                    //   onFocused: () {
                    //     print('focused - Category: ${widget.categories[5].name}');
                    //   },
                    // ),
                    MovieList(
                      title: widget.categories[5].name,
                      tiles: widget.categories[5].tiles,
                      columns: ColumnCount.four,
                      onFocused: () {
                        print('focused - Category: ${widget.categories[5].name}');
                        // _scrollController.animateTo(
                        //   index == 0
                        //       ? 570
                        //       : index == 1
                        //           ? 570 + 140
                        //           : 570 + 140 + ((index - 1) * 168),
                        //   duration: const Duration(milliseconds: 100),
                        //   curve: Curves.easeInQuad,
                        // );
                      },
                    ),
                    MovieList(
                      tiles: widget.categories[4].tiles,
                      columns: ColumnCount.three,
                      title: 'Youtube',
                      icon: 'assets/mock/images/icons8-youtube-144.png',
                      timeStamp: true,
                      onFocused: () {
                        debugPrint('Recently uploaded focused');
                        // _scrollController.animateTo(
                        //   570 + 140 + ((widget.categories.length - 1) * 168) + 233,
                        //   duration: const Duration(milliseconds: 100),
                        //   curve: Curves.easeInQuad,
                        // );
                      }
                    ),
                    ImportantInformation(
                      onFocused: (context) {
                        Scrollable.ensureVisible(
                          context,
                          alignment: 1,
                          duration: Duration(milliseconds: 100),
                          curve: Curves.easeInQuad
                        );
                      }
                    ),
                ]),
              )),
        ]),
      ],
    ));
  }
}
