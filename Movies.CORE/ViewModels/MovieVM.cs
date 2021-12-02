using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class MovieVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Trailer { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }
        public double AverageVote { get; set; }
        public int UserVote { get; set; }
        public List<GenreVM> Genres { get; set; }
        public List<MovieTheaterVM> MovieTheaters { get; set; }
        public List<ActorMovieVM> Actors { get; set; }

    }
}
