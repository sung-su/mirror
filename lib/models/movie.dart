import 'package:flutter/material.dart';
import 'package:dio/dio.dart';
import 'package:provider/provider.dart';

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
    }
    catch (e) {
      print('Error fetching movie($movieId) data: $e');
    }
    notifyListeners();
    return movie;
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
            'append_to_response': 'credits,reviews',
          },
        );
        print('response: ${response.data}');
        _movies.add(Movie.fromJson(response.data));
        //sampleMovies.add(Movie.fromJson(response.data));
      }
      catch (e) {
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
    }
    catch (e) {
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

  const Cast({
    required this.name,
    required this.originalName,
    required this.profilePath,
    required this.character,
  });

  factory Cast.fromJson(Map<String, dynamic> json) {
    return Cast(
      name: json['name'] ?? '',
      originalName: json['original_name'] ?? '',
      profilePath: json['profile_path'] ?? '',
      character: json['character'] ?? '',
    );
  }

  String get profileUrl {
    return profilePath.isNotEmpty ? 'https://image.tmdb.org/t/p/w500$profilePath' : '';
  }
}

class Crew {
  const Crew({
    required this.name,
    required this.originalName,
    required this.profilePath,
    required this.job,
  });
  final String name;
  final String originalName;
  final String profilePath;
  final String job;

  factory Crew.fromJson(Map<String, dynamic> json) {
    return Crew(
      name: json['name'] ?? '',
      originalName: json['original_name'] ?? '',
      profilePath: json['profile_path'] ?? '',
      job: json['job'] ?? '',
    );
  }

  String get profileUrl {
    return profilePath.isNotEmpty ? 'https://image.tmdb.org/t/p/w500$profilePath' : '';
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
  final int runtime;
  final List<Cast> cast;
  final List<Crew> crew;

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
    this.voteAverage = 0.0,
  });

  factory Movie.fromJson(Map<String, dynamic> json) {
    return Movie(
      title: json['title'],
      overview: json['overview'],
      posterPath: json['poster_path'],
      backdropPath: json['backdrop_path'],
      runtime: json['runtime'] ?? 0,
      releaseDate: json['release_date'],
      genres : (json['genres'] as List).map((e) => Genre.fromJson(e)).toList(),
      voteAverage: json['vote_average'],
      cast : (json['credits']['cast'] as List).map((e) => Cast.fromJson(e)).toList(),
      crew : (json['credits']['crew'] as List).map((e) => Crew.fromJson(e)).toList(),
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