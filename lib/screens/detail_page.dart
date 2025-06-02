import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/screens/detail_footer.dart';
import 'package:tizen_fs/screens/flexible_title_detail.dart';
import 'package:tizen_fs/screens/video_backdrop.dart';
import 'package:tizen_fs/screens/button_list.dart';
import 'package:tizen_fs/widgets/cast_list.dart';
import 'package:tizen_fs/screens/review_list.dart';
import 'package:tizen_fs/widgets/movie_list.dart';
import 'package:tizen_fs/widgets/youtube_list.dart';

class DetailPage extends StatefulWidget {
  final Movie movie;
  const DetailPage({super.key, required this.movie});

  @override
  State<DetailPage> createState() => _DetailPageState();
}

class _DetailPageState extends State<DetailPage> {
  final ScrollController _scrollController =
      ScrollController(initialScrollOffset: 240);
  final GlobalKey<ButtonListState> _scrollAnchor = GlobalKey<ButtonListState>();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final RenderBox renderBox =
          _scrollAnchor.currentContext!.findRenderObject() as RenderBox;
      _scrollAnchor.currentState
          ?.requestFocus(); // request focus on first element of the page
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
          RepaintBoundary(
            child: SizedBox.expand(
                child: VideoBackdrop(
                    scrollController: _scrollController,
                    imageUrl:
                        'https://media.themoviedb.org/t/p/w1066_and_h600_bestv2/${movie.backdropPath}',
                    videoUrl: 'assets/mock/videos/conclave_trailer.mp4')),
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
                      padding: const EdgeInsets.only(top: 20),
                      child: Column(
                        spacing: 10,
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          if (movie.reviews.isNotEmpty)
                            ReviewList(
                              reviews: movie.reviews,
                              onFocused: () {
                                _scrollController.animateTo(
                                  230, 
                                  duration: Duration(milliseconds: 100),
                                  curve: Curves.easeInQuad
                                );
                              }
                            ),
                          SizedBox(height: 10),
                          ButtonList(
                            key: _scrollAnchor,
                            onFocused: () {
                              _scrollController.animateTo(
                                230, 
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInQuad
                              );
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
                              //TODO: stop the video backdrop
                            }
                          ),
                          MovieList(
                            title: 'If you like ${movie.title} Movie',
                            similars: movie.similars,
                            onFocused: () {
                              _scrollController.animateTo(
                                660 + 175 * 1, 
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInQuad
                              );
                            },
                          ),
                          YoutubeList(
                            title: '${movie.title} Movie on YouTube',
                            videos: movie.videos,
                            onFocused: () {
                              _scrollController.animateTo(
                                660 + 175 * 2, 
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInQuad
                              );
                            },
                          ),
                          ImportantInformation(
                            onFocused: (context){
                              Scrollable.ensureVisible(
                                context,
                                alignment: 0.95,
                                duration: Duration(milliseconds: 100),
                                curve: Curves.easeInOut,
                              );
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
