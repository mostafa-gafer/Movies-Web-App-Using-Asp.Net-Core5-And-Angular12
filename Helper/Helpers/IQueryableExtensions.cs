using Movies.CORE.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> paginate<T>(this IQueryable<T> queryable, PaginationVM paginationVM)
        {
            return queryable.Skip((paginationVM.Page - 1) * paginationVM.RecordsPerPage)
                            .Take(paginationVM.RecordsPerPage);
        }
    }
}
