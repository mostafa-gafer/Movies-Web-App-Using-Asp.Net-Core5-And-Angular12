using Microsoft.EntityFrameworkCore;
using Movies.CORE.Context;
using Movies.CORE.Entities;
using Movies.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Repositories
{
    public class MovieRepository : BaseRepository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext context) : base(context)
        {
        }
        private ApplicationDbContext _DB => (ApplicationDbContext)_context;
        public async Task<Movie> GetMovieByIdAsync(int id)
        {
          return await _DB.Movies
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieTheatersMovies).ThenInclude(x => x.MovieTheater)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .FirstOrDefaultAsync(x => x.Id == id);

        }
    }
}
