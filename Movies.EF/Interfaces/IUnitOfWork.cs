using Movies.CORE.Context;
using Movies.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Genre> Genres { get; }

        IBaseRepository<Actor> Actors { get; }
        IBaseRepository<MovieTheater> MovieTheaters { get; }
        //IBaseRepository<Movie> Movies { get; }
        IBaseRepository<MoviesActors> MoviesActors { get; }
        IBaseRepository<MoviesGenres> MoviesGenres { get; }
        IBaseRepository<MovieTheatersMovies> MovieTheatersMovies { get; }
        IMovieRepository Movies { get; }
        IRatingRepository Ratings { get; }
        ApplicationDbContext DB { get; }
        int Complete();
    }
}
