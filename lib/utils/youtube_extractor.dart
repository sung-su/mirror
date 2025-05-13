import 'dart:convert';
import 'package:http/http.dart' as http;

class YouTubeExtractor {
  late List<Map<String, dynamic>> videos;
  final String _apiKey;
  final String _channelId;
  final int _maxResults;

  bool _initialized = false;

  YouTubeExtractor(
      {required String apiKey, required String channelId, int maxResults = 10})
      : _channelId = channelId,
        _apiKey = apiKey,
        _maxResults = maxResults;

  Future<void> initialize() async {
    if (_initialized) return;
    await _fetchContents();
    _initialized = true;
  }

  Future<void> _fetchContents() async {
    final apiUrl =
        'https://www.googleapis.com/youtube/v3/search?key=$_apiKey&channelId=$_channelId&part=snippet&type=video&order=date&maxResults=$_maxResults';
    final response = await http.get(Uri.parse(apiUrl));

    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      final videoItems = data['items'];
      final videoIds =
          videoItems.map((item) => item['id']['videoId']).join(',');
      final detailsUrl =
          'https://www.googleapis.com/youtube/v3/videos?key=$_apiKey&id=$videoIds&part=snippet,contentDetails,statistics';
      final detailsResponse = await http.get(Uri.parse(detailsUrl));

      if (detailsResponse.statusCode == 200) {
        final detailsData = json.decode(detailsResponse.body);
        videos = (detailsData['items'] as List).cast<Map<String, dynamic>>();
      }
    } else {
      throw Exception('Failed to load videos: ${response.body}');
    }
  }

  String formatDuration(String duration) {
    final RegExp regex = RegExp(r'PT(\d+H)?(\d+M)?(\d+S)?');
    final match = regex.firstMatch(duration);
    if (match == null) return '0:00';

    final hours = match.group(1)?.replaceAll('H', '') ?? '0';
    final minutes = match.group(2)?.replaceAll('M', '') ?? '0';
    final seconds = match.group(3)?.replaceAll('S', '') ?? '0';

    if (int.parse(hours) > 0) {
      return '$hours:${minutes.padLeft(2, '0')}:${seconds.padLeft(2, '0')}';
    } else {
      return '$minutes:${seconds.padLeft(2, '0')}';
    }
  }
}
