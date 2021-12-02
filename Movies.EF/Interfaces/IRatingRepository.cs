using Movies.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Interfaces
{
    public interface IRatingRepository: IBaseRepository<Rating>
    {
        Task<Rating> GetRatingAsync(int movieId, string userId);
    }
}
