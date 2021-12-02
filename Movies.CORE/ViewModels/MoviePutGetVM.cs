using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class MoviePutGetVM
    {
        public MovieVM Movie { get; set; }
        public List<GenreVM> SelectedGenres { get; set; }
        public List<GenreVM> NonSelectedGenres { get; set; }
        public List<MovieTheaterVM> SelectedMovieTheaters { get; set; }
        public List<MovieTheaterVM> NonSelectedMovieTheaters { get; set; }
        public List<ActorMovieVM> Actors { get; set; }
    }
}
