import 'package:flutter/material.dart';
import 'tile.dart';

class Category extends ChangeNotifier {
  final int uid;
  final String name;
  final List<Tile> tiles;

  int _selectedTileIndex = 0;

  Category({
    required this.uid,
    required this.name,
    required this.tiles,
  });

  int get selectedTileIndex => _selectedTileIndex;
  set selectedTileIndex(int index) {
    if (index != _selectedTileIndex) {
      _selectedTileIndex = index;
      notifyListeners();
    }
  }

  int get tileCount => tiles.length;
  Tile getTile(int index) {
    if (index < 0 || index >= tiles.length) {
      throw RangeError('Index out of range: $index');
    }
    return tiles[index];
  }

  Tile get selectedTile => tiles[_selectedTileIndex];
}
