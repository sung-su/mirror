import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/category.dart';
import 'package:tizen_fs/utils/media_db_parser.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';

class MediaDBParserPocPage extends StatelessWidget {
  const MediaDBParserPocPage({super.key});

  @override
  Widget build(BuildContext context) {
    return BackdropScaffold(
      child: MediaDBParserPoc(),
    );
  }
}

class MediaDBParserPoc extends StatefulWidget {
  const MediaDBParserPoc({super.key});

  @override
  State<MediaDBParserPoc> createState() => _MediaDBParserPocState();
}

class _MediaDBParserPocState extends State<MediaDBParserPoc> {
  List<Category> categories = [];
  bool _isFocused = false;
  final _focusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  Future<void> _loadData() async {
    final mediaDBParser = MediaDBParser();
    categories = await mediaDBParser.loadCategories();
    setState(() {
      categories = categories;
    });
  }

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      padding: const EdgeInsets.symmetric(vertical: 16),
      itemCount: categories.length,
      itemBuilder: (context, index) {
        final category = categories[index];
        return Column(
          children: [
            Container(
              alignment: Alignment.topLeft,
              padding: const EdgeInsets.only(left: 60, top: 10, bottom: 10),
              child: Text(category.name,
                  textAlign: TextAlign.left,
                  style: const TextStyle(
                    fontSize: 20,
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  )),
            ),
            Focus(
              focusNode: _focusNode,
              onFocusChange: (focus) => setState(() => _isFocused = focus),
              child: SizedBox(
                  height: 115,
                  child: ScrollConfiguration(
                    behavior: ScrollBehavior()
                        .copyWith(scrollbars: false, overscroll: false),
                    child: ListView.builder(
                      scrollDirection: Axis.horizontal,
                      padding: const EdgeInsets.symmetric(horizontal: 60),
                      itemCount: category.tiles.length,
                      itemBuilder: (context, index) {
                        final tile = category.tiles[index];
                        return Container(
                          width: 190,
                          margin: const EdgeInsets.only(right: 12),
                          decoration: BoxDecoration(
                            color: Colors.grey.shade900,
                            borderRadius: BorderRadius.circular(10),
                          ),
                          child: Stack(
                            fit: StackFit.expand,
                            children: [
                              ClipRRect(
                                borderRadius: BorderRadius.circular(10),
                                child: _buildTileImage(tile.iconUrl!),
                              ),
                              Positioned(
                                bottom: 0,
                                left: 8,
                                right: 8,
                                child: Text(
                                  tile.title,
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontSize: 12,
                                    fontWeight: FontWeight.bold,
                                    shadows: [
                                      Shadow(
                                        color: Colors.black,
                                        blurRadius: 2,
                                      )
                                    ],
                                  ),
                                  maxLines: 2,
                                  overflow: TextOverflow.ellipsis,
                                ),
                              ),
                            ],
                          ),
                        );
                      },
                    ),
                  )),
            ),
            const SizedBox(height: 20),
          ],
        );
      },
    );
  }

  Widget _buildTileImage(String iconUrl) {
    if (iconUrl.startsWith('http')) {
      return CachedNetworkImage(
        imageUrl: iconUrl,
        placeholder: (context, url) => const CircularProgressIndicator(),
        errorWidget: (context, url, error) => const Icon(Icons.error),
        fit: BoxFit.fill,
      );
    } else {
      return const Center(
          child: Icon(Icons.image_not_supported, color: Colors.grey));
    }
  }
}
