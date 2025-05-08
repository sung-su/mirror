class Tile {
  final int uid;
  final int parentUID;
  final String title;
  final String? iconUrl;
  final Map<String, dynamic> details;

  Tile({
    required this.uid,
    required this.parentUID,
    required this.title,
    required this.iconUrl,
    this.details = const {},
  });
}
