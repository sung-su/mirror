import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/media_db_parser.dart';
import 'package:tizen_fs/utils/youtube_extractor.dart';
import 'router.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  final mediaDBParser = MediaDBParser();
  mediaDBParser.initialize();

  const String apiKey = 'YOUTUBE_API_KEY'; // YouTube API Key
  const String channelId = 'UCWwgaK7x0_FR1goeSRazfsQ'; // Samsung Channel
  final youtubeExtractor =
      YouTubeExtractor(apiKey: apiKey, channelId: channelId);
  // Uncomment the following line to initialize the YouTube extractor
  //await youtubeExtractor.initialize();

  final MovieViewModel movieViewModel = MovieViewModel()..fetchSampleMovies();


  runApp(MultiProvider(
    providers: [
      ChangeNotifierProvider<MediaDBParser>.value(value: mediaDBParser),
      ChangeNotifierProvider<MovieViewModel>.value(value: movieViewModel),
      Provider<YouTubeExtractor>.value(value: youtubeExtractor),
    ],
    child: TizenFS(),
  ));
}

class MouseDraggableScrollBehavior extends ScrollBehavior {
  @override
  // Add mouse drag on desktop for easier responsive testing
  Set<PointerDeviceKind> get dragDevices {
    final devices = Set<PointerDeviceKind>.from(super.dragDevices);
    devices.add(PointerDeviceKind.mouse);
    return devices;
  }
}

class TizenFS extends StatelessWidget {
  const TizenFS({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Tizen First Screen',
      theme: $style.colors.toThemeData(),
      scrollBehavior: MouseDraggableScrollBehavior(),
      routerConfig: AppRouter.router,
    );
  }
}