import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:dio/dio.dart';
import 'package:path_provider/path_provider.dart';

class MovieViewModel extends ChangeNotifier {
  final Dio _dio = Dio();
  final String _apiKey = "cd67e26145a869a52a1822749ac0ae62";
  List<Movie> _movies = [];
  bool _isLoading = true;

  List<Movie> get movies => _movies;
  bool get isLoading => _isLoading;

  Future<Movie> fetchMovieDetailById(int movieId) async {
    Movie movie = Movie(
      title: '',
      overview: '',
      posterPath: '',
      backdropPath: '',
    );
    try {
      final response = await _dio.get(
        'https://api.themoviedb.org/3/movie/$movieId',
        queryParameters: {
          'api_key': _apiKey,
          'language': 'en-US',
        },
      );
      movie = Movie.fromJson(response.data);
    } catch (e) {
      print('Error fetching movie($movieId) data: $e');
    }
    notifyListeners();
    return movie;
  }

  Future<void> saveJson(Map<String, dynamic> jsonData) async {
    final dir = await getApplicationDocumentsDirectory();
    final file = File('${dir.path}/movie.json');
    String encodedData = jsonEncode(jsonData);
    await file.writeAsString(encodedData);
    print('filepath=[${file.path}]');
  }

  Future<void> fetchSampleMovies() async {
    final List<int> movieIds = [
      950387, // A Minecraft Movie
      822119, // Captain America: Brave New World
      575265, // Mission: Impossible
      974576, // CONCLAVE
    ];
    //final List<Movie> sampleMovies = [];

    for (int id in movieIds) {
      try {
        final response = await _dio.get(
          'https://api.themoviedb.org/3/movie/$id',
          queryParameters: {
            'api_key': _apiKey,
            'language': 'en-US',
            'append_to_response':
                'release_dates,videos,credits,reviews,similar,lists',
          },
        );
        var movie = Movie.fromJson(response.data);
        // await saveJson(response.data);
        _movies.add(movie);
        // _movies.add(Movie.fromJson(response.data));
        //sampleMovies.add(Movie.fromJson(response.data));
      } catch (e) {
        print('Error fetching movie($id) data: $e ');
      }
    }
    notifyListeners();
    //return sampleMovies;
    //return _movies;
  }

  Future<void> fetchPopularMovies() async {
    try {
      final response = await _dio.get(
        'https://api.themoviedb.org/3/movie/popular',
        queryParameters: {
          'api_key': _apiKey,
          'language': 'en-US',
        },
      );
      final List results = response.data['results'];
      _movies = results.map((json) => Movie.fromJson(json)).toList();
      notifyListeners();
    } catch (e) {
      print('Error fetching movies data: $e');
    }
  }
}

class Genre {
  final int id;
  final String name;

  const Genre({
    required this.id,
    required this.name,
  });

  factory Genre.fromJson(Map<String, dynamic> json) {
    return Genre(id: json['id'] ?? 0, name: json['name'] ?? '');
  }
}

class Cast {
  final String name;
  final String originalName;
  final String profilePath;
  final String character;
  final String department;

  const Cast({
    required this.name,
    required this.originalName,
    required this.profilePath,
    required this.character,
    required this.department,
  });

  factory Cast.fromJson(Map<String, dynamic> json) {
    return Cast(
      name: json['name'] ?? '',
      originalName: json['original_name'] ?? '',
      profilePath: json['profile_path'] ?? '',
      character: json['character'] ?? '',
      department: json['known_for_department'] ?? '',
    );
  }

  String get profileUrl {
    return profilePath.isNotEmpty
        ? 'https://image.tmdb.org/t/p/w500$profilePath'
        : '';
  }
}

class Crew {
  const Crew({
    required this.name,
    required this.originalName,
    required this.profilePath,
    required this.job,
    required this.department,
  });
  final String name;
  final String originalName;
  final String profilePath;
  final String job;
  final String department;

  factory Crew.fromJson(Map<String, dynamic> json) {
    return Crew(
      name: json['name'] ?? '',
      originalName: json['original_name'] ?? '',
      profilePath: json['profile_path'] ?? '',
      job: json['job'] ?? '',
      department: json['known_for_department'] ?? '',
    );
  }

  String get profileUrl {
    return profilePath.isNotEmpty
        ? 'https://image.tmdb.org/t/p/w500$profilePath'
        : '';
  }
}

class Reviews {
  const Reviews({
    required this.author,
    required this.content,
    required this.createdAt,
    required this.url,
  });
  final String author;
  final String content;
  final String createdAt;
  final String url;

  factory Reviews.fromJson(Map<String, dynamic> json) {
    return Reviews(
      author: json['author'] ?? '',
      content: json['content'] ?? '',
      createdAt: json['created_at'] ?? '',
      url: json['url'] ?? '',
    );
  }
}

class Video {
  const Video({
    required this.name,
    required this.key,
    required this.site,
    required this.size,
    required this.type,
    required this.official,
    required this.publishedAt,
  });
  final String name;
  final String key;
  final String site;
  final int size;
  final String type;
  final bool official;
  final String publishedAt;

  factory Video.fromJson(Map<String, dynamic> json) {
    return Video(
      name: json['name'] ?? '',
      key: json['key'] ?? '',
      site: json['site'] ?? '',
      size: json['size'] ?? 0,
      type: json['type'] ?? '',
      official: json['official'] ?? false,
      publishedAt: json['published_at'] ?? '',
    );
  }

  String get mp4url {
    return 'http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4';
  }

  String get youtubeUrl {
    if (site == 'YouTube' && key.isNotEmpty)
      return 'https://www.youtube.com/watch?v=$key';
    return '';
  }

  String get youtubeThumbnail {
    if (site == 'YouTube' && key.isNotEmpty)
      return 'https://img.youtube.com/vi/$key/0.jpg';
    return '';
  }

  String get publishedYear {
    return publishedAt.split('-')[0];
  }
}

class ReleaseDate {
  const ReleaseDate({
    required this.iso_639_1,
    required this.iso_3166_1,
    required this.certification,
    required this.releaseDate,
  });
  final String iso_639_1;
  final String iso_3166_1;
  final String certification;
  final String releaseDate;

  factory ReleaseDate.fromJson(Map<String, dynamic> json,
      {String iso31661 = 'US'}) {
    return ReleaseDate(
      iso_3166_1: iso31661 ?? '',
      iso_639_1: json['iso_639_1'] ?? '',
      certification: json['certification'] ?? '',
      releaseDate: json['release_date'] ?? '',
    );
  }
}

class Similar {
  final String title;
  final String overview;
  final String posterPath;
  final String backdropPath;
  final String releaseDate;

  Similar({
    required this.title,
    required this.overview,
    required this.posterPath,
    required this.backdropPath,
    this.releaseDate = '',
  });

  factory Similar.fromJson(Map<String, dynamic> json) {
    return Similar(
      title: json['title'] ?? '',
      overview: json['overview'] ?? '',
      posterPath: json['poster_path'] ?? '',
      backdropPath: json['backdrop_path'] ?? '',
      releaseDate: json['release_date'] ?? '',
    );
  }

  String get mp4url {
    return 'http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4';
  }

  String get posterUrl {
    return 'https://image.tmdb.org/t/p/w500$posterPath';
  }

  String get backdropUrl {
    return 'https://image.tmdb.org/t/p/w500$backdropPath';
  }

  String get releaseYear {
    return releaseDate.split('-')[0];
  }

  String get shortOverview {
    if (overview.length > 100) {
      return '${overview.substring(0, 100)}...';
    }
    return overview;
  }
}

class Movie {
  final String title;
  final String overview;
  final String posterPath;
  final String backdropPath;
  final String releaseDate;
  final List<Genre> genres;
  final double voteAverage;
  final int voteCount;
  final double popularity;
  final int runtime;
  final List<Cast> cast;
  final List<Crew> crew;
  final List<Reviews> reviews;
  final List<Video> videos;
  //final List<ReleaseDate> releaseDates;
  final String certification;
  final List<Similar> similars;

  Movie({
    required this.title,
    required this.overview,
    required this.posterPath,
    required this.backdropPath,
    this.releaseDate = '',
    this.genres = const [],
    this.voteCount = 0,
    this.runtime = 0,
    this.cast = const [],
    this.crew = const [],
    this.reviews = const [],
    this.voteAverage = 0.0,
    this.popularity = 0.0,
    this.videos = const [],
    //this.releaseDates = const [],
    this.certification = '',
    this.similars = const [],
  });

  factory Movie.fromJson(Map<String, dynamic> json) {
    return Movie(
      title: json['title'] ?? '',
      overview: json['overview'] ?? '',
      posterPath: json['poster_path'] ?? '',
      backdropPath: json['backdrop_path'] ?? '',
      runtime: json['runtime'] ?? 0,
      releaseDate: json['release_date'] ?? '',
      genres: (json['genres'] as List).map((e) => Genre.fromJson(e)).toList(),
      voteAverage: json['vote_average'] ?? 0,
      voteCount: json['vote_count'] ?? 0,
      popularity: json['popularity'] ?? 0,
      cast: (json['credits']['cast'] as List)
          .map((e) => Cast.fromJson(e))
          .toList(),
      crew: (json['credits']['crew'] as List)
          .map((e) => Crew.fromJson(e))
          .toList(),
      reviews: json['reviews']['total_results'] > 0
          ? (json['reviews']['results'] as List)
              .map((e) => Reviews.fromJson(e))
              .toList()
          : [],
      videos: (json['videos']['results'] as List)
          .map((e) => Video.fromJson(e))
          .toList(),
      // releaseDates: (json['release_dates']['results'] as List)
      //     .where((e) => e['iso_3166_1'] == 'US')
      //     .expand((e) => e['release_dates'] as List)
      //     .where((e) => e['iso_639_1'] == 'en' && e['certification'].isNotEmpty)
      //     .map((e) => ReleaseDate.fromJson(e, iso31661: 'US'))
      //     .toList(),
      certification: (json['release_dates']['results'] as List)
          .where((e) => e['iso_3166_1'] == 'US')
          .expand((e) => e['release_dates'] as List)
          .where((e) => e['iso_639_1'] == 'en' && e['certification'].isNotEmpty)
          .map((e) => e['certification'] ?? '')
          .toString(),
      similars: (json['similar']['results'] as List)
          .map((e) => Similar.fromJson(e))
          .toList(),
    );
  }

  String get posterUrl {
    return 'https://image.tmdb.org/t/p/w500$posterPath';
  }

  String get backdropUrl {
    return 'https://image.tmdb.org/t/p/w500$backdropPath';
  }

  String get releaseYear {
    return releaseDate.split('-')[0];
  }

  String get shortOverview {
    if (overview.length > 100) {
      return '${overview.substring(0, 100)}...';
    }
    return overview;
  }
}
