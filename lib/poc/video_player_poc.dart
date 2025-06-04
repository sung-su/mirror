import 'package:flutter/material.dart';
import 'package:video_player/video_player.dart';

class VideoPlayerPocPage extends StatelessWidget {
  const VideoPlayerPocPage({super.key});

  @override
  Widget build(BuildContext context) {
    return VideoScreen();
  }
}

class VideoScreen extends StatefulWidget {
  @override
  State<VideoScreen> createState() => _VideoScreenState();
}

class _VideoScreenState extends State<VideoScreen> {
  late VideoPlayerController _controller;

  final url = 'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4';
  final assetUrl = 'assets/mock/videos/conclave_trailer.mp4';


  bool isNetworkUrl(String input) {
    final RegExp urlRegex = RegExp(r'^(https?:)?\/\/[^\s]+$');
    return urlRegex.hasMatch(input);
  }

  @override
  void initState() {
    super.initState();
    _controller = isNetworkUrl(assetUrl) ? VideoPlayerController.networkUrl(Uri.parse(assetUrl)) : VideoPlayerController.asset(assetUrl)
      ..initialize().then((_) {
        setState(() {
          _controller.play();
        });
      });
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(55, 10, 55, 10),
      child: Stack(
        children: [
          Container(
            child: _controller.value.isInitialized ?
              AspectRatio(aspectRatio: _controller.value.aspectRatio,
                child: VideoPlayer(_controller),
              )
              : Container(
                child: Image.asset('assets/mock/images/conclave.png'),
              ),
          ),
          Positioned(
            bottom: 0,
            child: Container(
              color: Colors.black.withAlpha(70),
              child: SizedBox(
                width: 960,
                height: 200,
                child: Column(
                  children: [
                    Text(
                    'content description here...',
                    style: const TextStyle(color: Colors.white, fontSize: 24))
                  ]
                ),
              ),
            ),
          )
        ]
      ),
    );
  }
}
