import 'tile.dart';

class Category {
  final int uid;
  final String name;
  final List<Tile> tiles;

  Category({
    required this.uid,
    required this.name,
    required this.tiles,
  });
}
