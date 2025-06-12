import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/router.dart';
import 'package:tizen_fs/widgets/detail_footer.dart';
import 'package:tizen_fs/widgets/flexible_title_detail.dart';
import 'package:tizen_fs/widgets/rotten_rating.dart';
import 'package:tizen_fs/widgets/video_backdrop.dart';
import 'package:tizen_fs/widgets/action_list.dart';
import 'package:tizen_fs/widgets/age_rating.dart';
import 'package:tizen_fs/widgets/cast_list.dart';
import 'package:tizen_fs/widgets/review_list.dart';
import 'package:tizen_fs/widgets/movie_list.dart';
import 'package:tizen_fs/widgets/star_rating.dart';
import 'package:tizen_fs/widgets/youtube_list.dart';

class DetailPage extends StatefulWidget {
  const DetailPage({super.key, required this.movie});

  final Movie movie;

  @override
  State<DetailPage> createState() => _DetailPageState();
}

class _DetailPageState extends State<DetailPage> with RouteAware {
  final ScrollController _scrollController =
      ScrollController(initialScrollOffset: 240);
  final GlobalKey<ActionListState> _scrollAnchor = GlobalKey<ActionListState>();
  final GlobalKey<VideoBackdropState> _backdropController = GlobalKey<VideoBackdropState>();

  int _backdropState = 0;

  void _setBackdropState(int state) {
    if (_backdropState == state) return;

    _backdropController.currentState!.status = state;
    _backdropState = state;
  }

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _scrollAnchor.currentState
          ?.requestFocus(); // request focus on first element of the page
    });
  }

  @override
  void dispose() {
    _scrollController.dispose();
    routeObserver.unsubscribe(this);
    super.dispose();
  }

  @override
  void didPushNext() {
    _setBackdropState(3);
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    routeObserver.subscribe(this, ModalRoute.of(context)!);
  }

  @override
  Widget build(BuildContext context) {
    final Movie movie = widget.movie;
    return Scaffold(
        body: Stack(
        children: [
          RepaintBoundary(
            child: SizedBox.expand(
                child: VideoBackdrop(
                  key: _backdropController,
                  imageUrl: movie.backdropUrl,
                  videoUrl: movie.backdropVideoUrl,
                  onStatusChanged: (status){
                    if(status == 2) {
                      _scrollController.animateTo(
                        0,
                        duration: const Duration(milliseconds: 300),
                        curve: Curves.easeInQuad
                      );
                    }
                  },
                  )
                ),
          ),
          CustomScrollView(
              scrollBehavior:
                  ScrollBehavior().copyWith(scrollbars: false, overscroll: false),
                  physics: ClampingScrollPhysics(),
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
                          onFocused: (context) {})),
                ),
                SliverToBoxAdapter(
                    child: Padding(
                      padding: const EdgeInsets.only(top: 15),
                      child: Column(
                        spacing: 20,
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          Padding(
                            padding: EdgeInsets.only(left: 58),
                            child: Row(
                              spacing: 20,
                              children: [
                                RottenRating(
                                  rating: (movie.voteAverage * 10).floor(),
                                  onFocused: () {
                                    _scrollController.animateTo(
                                      250, 
                                      duration: Duration(milliseconds: 100),
                                      curve: Curves.easeInQuad
                                    );
                                    if(_backdropState != 0) {
                                      _setBackdropState(3);
                                    }
                                  }
                                ),
                                StarRating(rating: (movie.voteAverage / 2).toStringAsFixed(2)),
                                AgeRating(rating: movie.certification),
                                Text(movie.genres.isNotEmpty ? movie.genres[0].name : ''),
                                Text(movie.releaseYear),
                                Text(movie.runtime > 0
                                    ? '${(movie.runtime / 60).floor()}h ${movie.runtime % 60}m'
                                    : ''),
                              ],
                            ),
                          ),
                          if (movie.reviews.isNotEmpty)
                            ReviewList(
                              reviews: movie.reviews,
                              onFocused: () {
                                _scrollController.animateTo(
                                  250, 
                                  duration: Duration(milliseconds: 100),
                                  curve: Curves.easeInQuad
                                );
                                if(_backdropState != 0) {
                                  _setBackdropState(3);
                                }
                              }
                            ),
                          ActionList(
                            key: _scrollAnchor,
                            movie: movie,
                            onFocused: () {
                              _scrollController.animateTo(
                                250, 
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInQuad
                              );
                              if(_backdropState != 0) {
                                _setBackdropState(3);
                              }
                            }
                          ),
                          CastList(
                            title: 'Cast & Crew',
                            casts: movie.cast,
                            onFocused: () {
                              _scrollController.animateTo(
                                660, 
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInQuad
                              );
                              _setBackdropState(4);
                            }
                          ),
                          MovieList(
                            title: 'If you like ${movie.title}',
                            similars: movie.similars,
                            onFocused: () {
                              _scrollController.animateTo(
                                660 + 178 * 1, 
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInQuad
                              );
                              _setBackdropState(4);
                            },
                          ),
                          YoutubeList(
                            title: '${movie.title} on YouTube',
                            videos: movie.videos,
                            onFocused: () {
                              _scrollController.animateTo(
                                660 + 178 * 2, 
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInQuad
                              );
                              _setBackdropState(4);
                            },
                          ),
                          ImportantInformation(
                            movie: movie,
                            onFocused: (context){
                              Scrollable.ensureVisible(
                                context,
                                alignment: 0.95,
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInOut,
                              );
                              _setBackdropState(4);
                            },
                          ),
                          SizedBox(
                            height: 10,
                          ),
                      ]),
                )),
              ]),
        ],
    ));
  }
}
