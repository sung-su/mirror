import 'dart:math';
import 'package:flutter/material.dart';
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
import 'package:tizen_fs/widgets/star_rating.dart';

class DetailPage extends StatefulWidget {
  final Movie movie;
  const DetailPage({super.key, required this.movie});

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
                      movie: movie,
                      onFocused: (context) {
                        Scrollable.ensureVisible(
                          context,
                          alignment: 0.15,
                          duration: Duration(milliseconds: 100),
                          curve: Curves.easeInQuad
                        );
                      }
                    ),
                    IfYouLikeList(
                      onFocused: (context) {
                        Scrollable.ensureVisible(
                          context,
                          alignment: 0.15,
                          duration: Duration(milliseconds: 100),
                          curve: Curves.easeInQuad
                        );
                      }
                    ),
                    IfYouLikeList(
                      onFocused: (context) {
                        Scrollable.ensureVisible(
                          context,
                          alignment: 0.15,
                          duration: Duration(milliseconds: 100),
                          curve: Curves.easeInQuad
                        );
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
