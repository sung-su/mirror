import 'dart:convert';
import 'dart:io';
import 'package:flutter/services.dart';
import 'package:path/path.dart';
import 'package:sqflite/sqflite.dart';
import 'package:tizen_fs/models/category.dart';
import 'package:tizen_fs/models/tile.dart';

class MediaDBParser {
  static const String dbPath = 'assets/sqlite.db';
  //'/home/owner/apps_rw/com.samsung.tv.home.media/shared/trusted/sqlite.db';
  Database? _db;
  final List<String> excludedCategories = [
    'Anchor Row',
    'Genre Bookmark',
    'Explore More',
  ];

  Future<void> _ensureOpen() async {
    if (_db == null || !_db!.isOpen) {
      final databasesPath = await getDatabasesPath();
      final path = join(databasesPath, 'sqlite.db');
      var exists = await databaseExists(path);

      if (!exists) {
        // The database does not exist, so copy it from the assets folder
        ByteData data = await rootBundle.load(dbPath);
        final bytes =
            data.buffer.asUint8List(data.offsetInBytes, data.lengthInBytes);
        await File(path).writeAsBytes(bytes, flush: false);
      }
      _db = await openDatabase(path, readOnly: true);
    }
  }

  Future<void> close() async {
    if (_db != null || _db!.isOpen) {
      await _db!.close();
      _db = null;
    }
  }

  Future<List<Category>> loadCategories() async {
    await _ensureOpen();
    final db = _db!;
    final List<Map<String, dynamic>> rows = await db.query(
      'TableEntity',
      where: 'visible = ?',
      whereArgs: [1],
    );

    final Map<int, Map<String, dynamic>> entryMap = {
      for (var row in rows) row['UID'] as int: row,
    };

    final Map<String, Category> categoryMap = {};

    for (var row in rows) {
      final String elementID = row['elementID'];
      final int uid = row['UID'];
      final int parentUID = row['parentUID'];
      final String? childEntityList = row['childEntityList'];
      final Map<String, dynamic> uiJson = _parseJson(row['UIJson']);

      String? title;
      if (elementID == 'd/dhcategory.layer') {
        title = uiJson['title'];
      } else if (elementID == 'd/webapp.layer') {
        final parentRow = entryMap[parentUID];
        final parentUIJson =
            parentRow != null ? _parseJson(parentRow['UIJson']) : {};
        title = parentUIJson['title'];
      }

      if (title != null &&
          title.trim().isNotEmpty &&
          !excludedCategories.contains(title)) {
        final childUIDs = _parseChildEntityList(childEntityList);
        final tiles = _loadTiles(childUIDs, entryMap);
        if (tiles.isNotEmpty) {
          if (categoryMap.containsKey(title)) {
            categoryMap[title]!.tiles.addAll(tiles);
          } else {
            categoryMap[title] = Category(uid: uid, name: title, tiles: tiles);
          }
        }
      }
    }
    return categoryMap.values.toList();
  }

  Future<void> printCategories() async {
    final categories = await loadCategories();

    print('=== Categories loaded: ${categories.length} ===');

    for (var category in categories) {
      print('Category: ${category.name} (UID: ${category.uid})');
      print('   Contains tiles: ${category.tiles.length}');
      for (var tile in category.tiles) {
        print('  Tile - Title: ${tile.title} (UID: ${tile.uid})');
        print('         Icon: ${tile.iconUrl ?? "None"}');

        if (tile.details.isNotEmpty) {
          print('       Details:');
          tile.details.forEach((key, value) {
            print('    $key: $value');
          });
        }
      }
      print('');
    }
    print('=== Finished loading categories ===');
  }

  List<Tile> _loadTiles(
      List<int> childUIDs, Map<int, Map<String, dynamic>> entryMap) {
    return childUIDs
        .map((uid) {
          final row = entryMap[uid];
          if (row == null) return null;

          final elementID = row['elementID'];
          final parentUID = row['parentUID'];
          final uiJson = _parseJson(row['UIJson']);
          final icon = uiJson['focused_icon'] ?? 'Unknown';
          final title = uiJson['title'] ?? 'Unknown';

          if (elementID == 'd/dhcategory.layer.tile') {
            final contentDetail =
                Map<String, dynamic>.from(uiJson['content_detail'] ?? {});
            return Tile(
                uid: uid,
                parentUID: parentUID,
                title: title,
                iconUrl: icon,
                details: contentDetail);
          } else {
            return Tile(
                iconUrl: icon, parentUID: parentUID, title: title, uid: uid);
          }
        })
        .whereType<Tile>()
        .toList();
  }

  Map<String, dynamic> _parseJson(String? jsonString) {
    try {
      return jsonString != null
          ? jsonDecode(jsonString) as Map<String, dynamic>
          : {};
    } catch (e) {
      print('Error parsing JSON: $e');
      return {};
    }
  }

  List<int> _parseChildEntityList(String? childEntityList) {
    if (childEntityList == null || childEntityList.isEmpty) return [];
    return childEntityList
        .split('_')
        .map((e) => int.tryParse(e) ?? -1)
        .where((id) => id >= 0)
        .toList();
  }
}
