using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movies.CORE.Context;
using Movies.CORE.Entities;
using Movies.CORE.ViewModels;
using Movies.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public RatingsController(UserManager<IdentityUser> userManager,
            IUnitOfWork unitOfWork,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _context = context;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Post([FromBody] RatingVM ratingVM)
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
            var user = await _userManager.FindByEmailAsync(email);
            var userId = user.Id;
            var currentRate = await _unitOfWork.Ratings.GetRatingAsync(ratingVM.MovieId, userId);
            if(currentRate == null)
            {
                var rating = new Rating();
                rating.MovieId = ratingVM.MovieId;
                rating.Rate = ratingVM.Rating;
                rating.UserId = userId;
                //await _unitOfWork.Ratings.AddAsync(rating);
                _context.Add(rating);

            }
            else
            {
                currentRate.Rate = ratingVM.Rating;
            }
            //_unitOfWork.Complete();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
