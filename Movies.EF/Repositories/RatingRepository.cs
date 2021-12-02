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
    public class RatingRepository : BaseRepository<Rating>, IRatingRepository
    {
        private readonly ApplicationDbContext context;

        public RatingRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        //private ApplicationDbContext _DB => (ApplicationDbContext)_context;
        public async Task<Rating> GetRatingAsync(int movieId, string userId) => await context.Ratings.FirstOrDefaultAsync(x => x.MovieId == movieId && x.UserId == userId);
        
    }
}
