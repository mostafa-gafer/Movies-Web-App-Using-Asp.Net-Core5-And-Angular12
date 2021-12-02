using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class HomeVM
    {
        public List<MovieVM> InTheaters { get; set; }
        public List<MovieVM> UpcomingReleases { get; set; }
    }
}
