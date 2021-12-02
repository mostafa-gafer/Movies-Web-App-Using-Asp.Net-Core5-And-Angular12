using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class MoviePostGetVM
    {
        public List<GenreVM> Genres { get; set; }
        public List<MovieTheaterVM> MovieTheaters  { get; set; }
    }
}
