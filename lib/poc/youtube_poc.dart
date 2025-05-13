import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:timeago/timeago.dart' as timeago;
import 'package:tizen_fs/utils/youtube_extractor.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';

class YoutubePocPage extends StatelessWidget {
  const YoutubePocPage({super.key});

  @override
  Widget build(BuildContext context) {
    return BackdropScaffold(
      child: YoutubePoc(),
    );
  }
}

class YoutubePoc extends StatefulWidget {
  const YoutubePoc({super.key});

  @override
  State<YoutubePoc> createState() => _YoutubePocState();
}

class _YoutubePocState extends State<YoutubePoc> {
  List<Map<String, dynamic>> _videos = [];

  @override
  void initState() {
    super.initState();
    _videos = Provider.of<YouTubeExtractor>(context, listen: false).videos;
    setState(() {
      _videos = _videos;
    });
  }

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      itemCount: _videos.length,
      itemBuilder: (context, index) {
        final video = _videos[index];
        final snippet = video['snippet'];
        final stats = video['statistics'];
        final contentDetails = video['contentDetails'];
        final videoId = video['id'];

        final publishedAt = DateTime.parse(snippet['publishedAt']);
        final publishedAgo = timeago.format(publishedAt, locale: 'en_short');

        return ListTile(
          contentPadding: const EdgeInsets.all(10),
          leading: Image.network(snippet['thumbnails']['high']['url'],
              width: 100, fit: BoxFit.cover),
          title: Text(snippet['title'], maxLines: 2),
          subtitle: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('${snippet['channelTitle']}'),
              Text(
                  '$publishedAgo • ${stats['viewCount']} views • ${Provider.of<YouTubeExtractor>(context, listen: false).formatDuration(contentDetails['duration'])}'),
            ],
          ),
          onTap: () {
            print('Video ID: ${video['title']} tapped');
            //Launch YouTube video
          },
        );
      },
    );
  }
}
