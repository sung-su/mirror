class Tile {
  final int uid;
  final int parentUID;
  final String title;
  final String? iconUrl;
  final int? imageWidth;
  final Map<String, dynamic> details;

  Tile({
    required this.uid,
    required this.parentUID,
    required this.title,
    required this.iconUrl,
    this.imageWidth,
    this.details = const {},
  });
}
