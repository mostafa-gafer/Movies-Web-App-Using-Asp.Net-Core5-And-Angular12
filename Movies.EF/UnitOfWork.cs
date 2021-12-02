using Microsoft.AspNetCore.Http;
using Movies.CORE.Context;
using Movies.CORE.Entities;
using Movies.EF.Interfaces;
using Movies.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        public IBaseRepository<Genre> Genres { get; private set; }
        public IBaseRepository<Actor> Actors { get; private set; }
        public IBaseRepository<MovieTheater> MovieTheaters { get; private set; }
        // public IBaseRepository<Movie> Movies { get; private set; }
        public IMovieRepository Movies { get; private set; }
        public IRatingRepository Ratings { get; private set; }
        public ApplicationDbContext DB { get; private set; }
        public IBaseRepository<MoviesActors> MoviesActors { get; private set; }
        public IBaseRepository<MoviesGenres> MoviesGenres { get; private set; }
        public IBaseRepository<MovieTheatersMovies> MovieTheatersMovies { get; private set; }

        public readonly ApplicationDbContext _context ;
        private readonly IHttpContextAccessor _CC;
        public UnitOfWork(ApplicationDbContext context , IHttpContextAccessor CC)
        {
            _context = context;
            Genres = new BaseRepository<Genre>(_context, _CC); //to make initialization in ctor

            Actors = new BaseRepository<Actor>(_context, _CC); //to make initialization in ctor

            MovieTheaters = new BaseRepository<MovieTheater>(_context, _CC); //to make initialization in ctor

            //Movies = new BaseRepository<Movie>(_context, _CC);
            
            Movies = new MovieRepository(_context);

            Ratings = new RatingRepository(context);

            MoviesActors = new BaseRepository<MoviesActors>(_context, _CC);
            
            MoviesGenres = new BaseRepository<MoviesGenres>(_context, _CC);
            
            MovieTheatersMovies = new BaseRepository<MovieTheatersMovies>(_context, _CC);
        }
        public void Dispose() => _context.Dispose();
        
        //To save changes in DB
        public int Complete() => _context.SaveChanges();   
        
        
    }
}
