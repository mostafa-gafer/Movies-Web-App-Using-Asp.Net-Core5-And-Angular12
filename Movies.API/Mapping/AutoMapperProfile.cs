using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Movies.CORE.Entities;
using Movies.CORE.ViewModels;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API.Mapping
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile(GeometryFactory geometryFactory)
        {
            
            CreateMap<Genre, GenreVM>()
                     .ReverseMap();

            CreateMap<Actor, ActorVM>()
                     .ReverseMap();

            CreateMap<ActorCreationVM, Actor>().ForMember(x => x.Picture, options => options.Ignore());

            //to mapper moviestheater with location point
            CreateMap<MovieTheater, MovieTheaterVM>()
                .ForMember(x => x.Latitude, dto => dto.MapFrom(prop => prop.Location.Y))
                .ForMember(x => x.Longitude, dto => dto.MapFrom(prop => prop.Location.X));
            CreateMap<MovieTheaterVM, MovieTheater>()
                .ForMember(x => x.Location, x => x.MapFrom(dto =>
                geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude))));

            CreateMap<MovieCreateVM, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheatersMovies, options => options.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieVM>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheaters, options => options.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));

            CreateMap<IdentityUser, UserVM>();

        }
        private List<ActorMovieVM> MapMoviesActors(Movie movie, MovieVM movieVM)
        {
            var result = new List<ActorMovieVM>();
            if (movie.MoviesActors == null)
                return result;

            foreach (var MoviesActors in movie.MoviesActors)
                result.Add(new ActorMovieVM() {
                    Id = MoviesActors.ActorId,
                    Name = MoviesActors.Actor.Name,
                    Character = MoviesActors.Character,
                    Picture = MoviesActors.Actor.Picture,
                    Order = MoviesActors.Order
                });

            return result;
        }
        private List<MovieTheaterVM> MapMovieTheatersMovies(Movie movie, MovieVM movieVM)
        {
            var result = new List<MovieTheaterVM>();
            if (movie.MovieTheatersMovies == null)
                return result;

            foreach (var MovieTheatersMovies in movie.MovieTheatersMovies)
                result.Add(new MovieTheaterVM() {
                    Id= MovieTheatersMovies.MovieTheaterId,
                    Name = MovieTheatersMovies.MovieTheater.Name,
                    Latitude = MovieTheatersMovies.MovieTheater.Location.Y,
                    Longitude = MovieTheatersMovies.MovieTheater.Location.X
                });

            return result;
        }
        private List<GenreVM> MapMoviesGenres(Movie movie, MovieVM movieVM)
        {
            var result = new List<GenreVM>();
            if (movie.MoviesGenres == null)
                return result;

            foreach (var genre in movie.MoviesGenres)
                result.Add(new GenreVM() { Id = genre.GenreId, Name = genre.Genre.Name});

            return result;
        }
        private List<MoviesGenres> MapMoviesGenres(MovieCreateVM movieVM, Movie movie)
        {
            var result = new List<MoviesGenres>();
            if (movieVM.GenresIds == null)
                return result;

            foreach(var id in movieVM.GenresIds)
                 result.Add(new MoviesGenres() { GenreId = id });

            return result;
        }
        private List<MovieTheatersMovies> MapMovieTheatersMovies(MovieCreateVM movieVM, Movie movie)
        {
            var result = new List<MovieTheatersMovies>();
            if (movieVM.MovieTheatersIds == null)
                return result;

            foreach (var id in movieVM.MovieTheatersIds)
                result.Add(new MovieTheatersMovies() { MovieTheaterId = id });

            return result;
        }
        private List<MoviesActors> MapMoviesActors(MovieCreateVM movieVM, Movie movie)
        {
            var result = new List<MoviesActors>();
            if (movieVM.Actors == null)
                return result;

            foreach (var actor in movieVM.Actors)
                result.Add(new MoviesActors() { ActorId = actor.Id, Character = actor.Character});

            return result;
        }
    }
}
