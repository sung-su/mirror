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
          },
        );
        print('response: ${response.data}');
        _movies.add(Movie.fromJson(response.data));
        //sampleMovies.add(Movie.fromJson(response.data));
      }
      catch (e) {
        print('Error fetching movie($id) data: $e');
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
  const Genre({
    required this.id,
    required this.name,
  });
  final int id;
  final String name;
}

class Movie {
  final String title;
  final String overview;
  final String posterPath;
  final String backdropPath;
  final String releaseDate;
  final Genre genres;

  Movie({
    required this.title,
    required this.overview,
    required this.posterPath,
    required this.backdropPath,
    this.releaseDate = '',
    this.genres = const Genre(id: 0, name: ''),
    double voteAverage = 0.0,
  });

  factory Movie.fromJson(Map<String, dynamic> json) {
    return Movie(
      title: json['title'],
      overview: json['overview'],
      posterPath: json['poster_path'],
      backdropPath: json['backdrop_path'],
      releaseDate: json['release_date'],
      voteAverage: json['vote_average'],
    );
  }

  String get posterUrl {
    return 'https://image.tmdb.org/t/p/w500$posterPath';
  }

  String get backdropUrl {
    return 'https://image.tmdb.org/t/p/w500$backdropPath';
  }

  String get shortOverview {
    if (overview.length > 100) {
      return '${overview.substring(0, 100)}...';
    }
    return overview;
  }
}