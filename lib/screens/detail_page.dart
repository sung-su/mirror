import 'dart:math';
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/screens/video_backdrop.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/age_rating.dart';
import 'package:tizen_fs/widgets/rotten_rating.dart';
import 'package:tizen_fs/widgets/star_rating.dart';

class DetailPage extends StatefulWidget {
  final Movie movie;
  const DetailPage({super.key, required this.movie});

  @override
  State<DetailPage> createState() => _DetailPageState();
}

class _DetailPageState extends State<DetailPage> {
  final ScrollController _scrollController =
      ScrollController(initialScrollOffset: 240);

  final GlobalKey _scrollAnchor = GlobalKey();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final RenderBox renderBox =
          _scrollAnchor.currentContext!.findRenderObject() as RenderBox;
      final offset = renderBox.localToGlobal(Offset.zero).dy +
          _scrollController.offset -
          (MediaQuery.of(context).size.height - renderBox.size.height - 50);
      _scrollController.jumpTo(offset);
    });
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
        CustomScrollView(controller: _scrollController, slivers: <Widget>[
          SliverAppBar(
            backgroundColor: Colors.transparent,
            automaticallyImplyLeading: false,
            toolbarHeight: 250,
            expandedHeight: MediaQuery.of(context).size.height - 50,
            flexibleSpace: Padding(
              padding: const EdgeInsets.only(
                left: 58,
              ),
              child: FlexibleTitleForDetail(
                  expandedHeight: MediaQuery.of(context).size.height - 50,
                  collapsedHeight: 250,
                  child: Text(movie.title)),
            ),
          ),
          SliverToBoxAdapter(
              child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                Padding(
                  padding: const EdgeInsets.only(left: 58, top: 16),
                  child: Row(
                    spacing: 20,
                    children: [
                      RottenRating(rating: 93),
                      StarRating(rating: 3.8),
                      AgeRating(rating: "12"),
                      Text(movie.genres.isNotEmpty ? movie.genres[0].name : ''),
                      Text(movie.releaseYear),
                      Text(movie.runtime > 0
                           ? '${(movie.runtime / 60).floor()}h ${movie.runtime % 60}m'
                           : ''),
                    ],
                  ),
                ),
                SizedBox(height: 20),
                if (movie.reviews.isNotEmpty)
                  MockReviewList(reviews: movie.reviews),
                SizedBox(height: 20),
                ButtonList(key: _scrollAnchor),
                SizedBox(height: 20),
                CastList(movie: movie),
                SizedBox(height: 20),
                IfYouLikeList(),
                SizedBox(height: 20),
                IfYouLikeList(),
                SizedBox(height: 20),
                Padding(
                  padding: const EdgeInsets.only(left: 58, right: 58),
                  child: Container(
                    height: 0.7,
                    color: Colors.white.withAlphaF(0.9),
                  ),
                ),
                ImportantInformation(),
                SizedBox(height: 100)
              ])),
        ]),
      ],
    ));
  }
}

class MockReviewList extends StatelessWidget {
  const MockReviewList({
    super.key,
    required this.reviews,
  });
  final List<Reviews> reviews;

  @override
  Widget build(BuildContext context) {
    return Container(
        padding: const EdgeInsets.only(left: 58, right: 58),
        color: Colors.grey,
        child: SingleChildScrollView(
          clipBehavior: Clip.none,
          scrollDirection: Axis.horizontal,
          child:
          Row(
            children: [
              ...List.generate(
                  reviews.length,
                  (index) {
                    final review = reviews[index];
                    return Card(
                      child: SizedBox(
                        height: 120,
                        width: 400,
                        child: Padding(
                          padding: const EdgeInsets.all(10),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(review.author, style: const TextStyle(fontSize: 15)),
                              const SizedBox(height: 5),
                              Text(review.content,
                                  maxLines: 3,
                                  overflow: TextOverflow.ellipsis,
                                  style: const TextStyle(fontSize: 12)),
                            ],
                          ),
                        ),
                      ),
                    );
                  }),
            ],
          )
        ));
  }
}

class ButtonList extends StatelessWidget {
  const ButtonList({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
        padding: const EdgeInsets.only(left: 58, right: 58),
        color: Colors.grey,
        child: SingleChildScrollView(
          clipBehavior: Clip.none,
          scrollDirection: Axis.horizontal,
          child: Row(spacing: 10, children: [
            FilledButton.icon(
              onPressed: () {},
              icon: Icon(Icons.bookmark_border_outlined),
              label: Text('Watchlist'),
            ),
            FilledButton.icon(
              onPressed: () {},
              icon: Icon(Icons.radio_button_off_outlined),
              label: Text('Watched it?'),
            ),
            IconButton.filled(
                onPressed: () {}, icon: Icon(Icons.format_quote_outlined)),
          ]),
        ));
  }
}

class CastList extends StatelessWidget {
  const CastList({
    super.key,
    required this.movie,
  });

  final Movie movie;

  @override
  Widget build(BuildContext context) {
    return Container(
        padding: const EdgeInsets.only(left: 58, right: 58),
        color: Colors.grey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text("Cast & Crew", style: TextStyle(fontSize: 20)),
            SingleChildScrollView(
              clipBehavior: Clip.none,
              scrollDirection: Axis.horizontal,
              child: Row(
                spacing: 10,
                children:
                  List.generate(movie.cast.length,
                      (index) {
                        return Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 10),
                          child: Column(
                            children: [
                              CircleAvatar(
                                radius: 60,
                                backgroundImage: NetworkImage(
                                    movie.cast.isNotEmpty
                                        ? 'https://media.themoviedb.org/t/p/w500${movie.cast[index].profilePath}'
                                        : ''),
                              ),
                              const SizedBox(height: 5),
                              Text(movie.cast[index].name,
                                  style: TextStyle(fontSize: 12)),
                              const SizedBox(height: 5),
                              Text(movie.cast[index].character,
                                  style: TextStyle(fontSize: 10)),
                            ],
                          ),
                        );
                      },
                  ),
                ),
            )],
        ));
  }
}

class IfYouLikeList extends StatelessWidget {
  const IfYouLikeList({
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
        padding: const EdgeInsets.only(left: 58, right: 58),
        color: Colors.grey,
        child: SingleChildScrollView(
          clipBehavior: Clip.none,
          scrollDirection: Axis.horizontal,
          child: Row(spacing: 10, children: [
            ...List.generate(
                10,
                (index) => Card.outlined(
                        child: SizedBox(
                      height: 150,
                      width: 200,
                    ))),
          ]),
        ));
  }
}

class ImportantInformation extends StatelessWidget {
  const ImportantInformation({super.key});
  @override
  Widget build(BuildContext context) {
    return Padding(
        padding: const EdgeInsets.symmetric(horizontal: 58, vertical: 20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          spacing: 10,
          children: [
            Text("Important Information", style: TextStyle(fontSize: 20)),
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              spacing: 20,
              children: [
                SizedBox(
                  width: 200,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    spacing: 10,
                    children: [
                      Text("Quality", style: TextStyle(fontSize: 15)),
                      Text(
                          "Automatically plays in the highjest\nquality available for your purchase",
                          style: TextStyle(fontSize: 12)),
                      Text("Purchase details", style: TextStyle(fontSize: 15)),
                      Text(
                          "Automatically plays in the highjest\nquality available for your purchase",
                          style: TextStyle(fontSize: 12)),
                    ],
                  ),
                ),
                SizedBox(
                  width: 200,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    spacing: 10,
                    children: [
                      Text("Rating description",
                          style: TextStyle(fontSize: 15)),
                      Text("15", style: TextStyle(fontSize: 12)),
                      Text("Data sharing", style: TextStyle(fontSize: 15)),
                      Text(
                          "Information about movie and show transactions may be shared amongst YouTube, Google TV, Google Play Movies & TV, and other Google Services to support your access to content and those services",
                          style: TextStyle(fontSize: 12)),
                    ],
                  ),
                )
              ],
            ),
          ],
        ));
  }
}

class FlexibleTitleForDetail extends StatelessWidget {
  final double expandedHeight;
  final double collapsedHeight;
  final Widget child;
  final double minFontSize;
  final double maxFontSize;

  const FlexibleTitleForDetail({
    super.key,
    required this.expandedHeight,
    required this.collapsedHeight,
    required this.child,
    this.minFontSize = 40,
    this.maxFontSize = 70,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(builder: (context, constraints) {
      final ratio = max(constraints.maxHeight - collapsedHeight, 0) /
          max(expandedHeight - collapsedHeight, 0);
      return Align(
          alignment: Alignment.bottomLeft,
          child: DefaultTextStyle(
              style: TextStyle(
                  fontSize: maxFontSize - ratio * (maxFontSize - minFontSize)),
              child: child));
    });
  }
}
