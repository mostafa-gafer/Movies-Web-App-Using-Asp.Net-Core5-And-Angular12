using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.ViewModels
{
    public class FilterMovieVM
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationVM PaginationVM {
            get { return new PaginationVM() { Page = Page, RecordsPerPage = RecordsPerPage }; }
        }
        public string Title { get; set; }
        public int GenreId { get; set; }
        public bool InTheaters { get; set; }
        public bool UpComingReleases { get; set; }
    }
}
