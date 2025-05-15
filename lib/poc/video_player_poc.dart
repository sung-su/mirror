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

  @override
  void initState() {
    super.initState();
    _controller = VideoPlayerController.asset('assets/mock/videos/conclave_trailer.mp4')
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
      child: _controller.value.isInitialized ? 
        AspectRatio(aspectRatio: _controller.value.aspectRatio,
          child: VideoPlayer(_controller),
        )
        : Container(
          child: Image.asset('assets/mock/images/conclave.png'),
        ),
    );
  }
}
