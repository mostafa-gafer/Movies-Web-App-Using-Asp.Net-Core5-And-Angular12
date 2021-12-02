using AutoMapper;
using Helper.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Movies.CORE.Context;
using Movies.CORE.Entities;
using Movies.CORE.ViewModels;
using Movies.EF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class MoviesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MovieTheatersController> _logger;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "Movies";
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public MoviesController(ApplicationDbContext context,
            IUnitOfWork unitOfWork,
            ILogger<MovieTheatersController> logger, 
            IMapper mapper, 
            IFileStorageService fileStorageService,
            UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<HomeVM>> Get()
        {
            var top = 6;
            var today = DateTime.Today;
            var upcomingReleases = await _context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();
            var inTheaters = await _context.Movies
                .Where(x => x.InTheaters)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();
            var homeVM = new HomeVM();
            homeVM.UpcomingReleases = _mapper.Map<List<MovieVM>>(upcomingReleases);
            homeVM.InTheaters = _mapper.Map<List<MovieVM>>(inTheaters);
            return homeVM;
        }
        [HttpGet("PutGet/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<MoviePutGetVM>> PutGet(int id)
        {
            var movieActionResult = await Get(id);
            if (movieActionResult.Result is NotFoundResult)
                return NotFound();

            var movie = movieActionResult.Value;
            var genresSelectedIds = movie.Genres.Select(x => x.Id).ToList();
            var nonSelectedGenres = await _context.Genres.Where(x => !genresSelectedIds.Contains(x.Id)).ToListAsync();

            var movieTheatersIds = movie.MovieTheaters.Select(x => x.Id).ToList();
            var nonSelectedMovieTheaters = await _context.MovieTheaters.Where(x =>
            !movieTheatersIds.Contains(x.Id)).ToListAsync();

            var nonSelectedGenresVMs = _mapper.Map<List<GenreVM>>(nonSelectedGenres);
            var nonSelectedMovieTheatersVM = _mapper.Map<List<MovieTheaterVM>>(nonSelectedMovieTheaters);

            var response = new MoviePutGetVM();
            response.Movie = movie;
            response.SelectedGenres = movie.Genres;
            response.NonSelectedGenres = nonSelectedGenresVMs;
            response.SelectedMovieTheaters = movie.MovieTheaters;
            response.NonSelectedMovieTheaters = nonSelectedMovieTheatersVM;
            response.Actors = movie.Actors;
            return response;
        }
        [HttpGet("Filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MovieVM>>> Filter([FromQuery] FilterMovieVM filterMovieVM)
        {
            var moviesQueryable = _context.Movies.AsQueryable();
            if (!string.IsNullOrEmpty(filterMovieVM.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMovieVM.Title));
            }
            if (filterMovieVM.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }
            if (filterMovieVM.UpComingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }
            if (filterMovieVM.GenreId != 0)
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(y => y.GenreId).Contains(filterMovieVM.GenreId));
            }
            await HttpContext.InsertParamtersPaginationInHeader(moviesQueryable);
            var movies = await moviesQueryable.OrderBy(x => x.Title).paginate(filterMovieVM.PaginationVM).ToListAsync();
            return _mapper.Map<List<MovieVM>>(movies);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromForm] MovieCreateVM model)
        {
            try
            {
                var movie = await _context.Movies.Include(x => x.MoviesActors)
                        .Include(x => x.MoviesGenres)
                        .Include(x => x.MovieTheatersMovies)
                        .FirstOrDefaultAsync(x => x.Id == id);
                if (movie == null)
                    return NotFound();

                movie = _mapper.Map(model, movie);
                if (model.Poster != null)
                    movie.Poster = await _fileStorageService.EditFile(containerName, model.Poster, movie.Poster);

                AnnotateActorsOrder(movie);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }

        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieVM>> Get(int id)
        {
            //var movie = await _unitOfWork.Movies.GetMovieByIdAsync(id);
            var movie = await _context.Movies
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieTheatersMovies).ThenInclude(x => x.MovieTheater)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
                return NotFound();

            var averageVote = 0.0;
            var userVote = 0;
            if(await _context.Ratings.AnyAsync(x => x.MovieId == id))
            {
                averageVote = await _context.Ratings.Where(x => x.MovieId == id).AverageAsync(x => x.Rate);
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
                    var user = await _userManager.FindByEmailAsync(email);
                    var userId = user.Id;
                    var ratingDb = await _context.Ratings.FirstOrDefaultAsync(x => x.MovieId == id && x.UserId == userId);

                    if (ratingDb != null)
                        userVote = ratingDb.Rate;

                }
            }
            var result = _mapper.Map<MovieVM>(movie);
            result.AverageVote = averageVote;
            result.UserVote = userVote;
            result.Actors = result.Actors.OrderBy(x => x.Order).ToList();
            return result;
        }
        [HttpGet("PostGet")]
        public async Task<ActionResult<MoviePostGetVM>> PostGet()
        {
            var movieTheaters = await _unitOfWork.MovieTheaters.GetAllAsync();
            movieTheaters.OrderBy(o => o.Name);
            var genres = await _unitOfWork.Genres.GetAllAsync();
            genres.OrderBy(o => o.Name);
            var movieTheatersVM = _mapper.Map<List<MovieTheaterVM>>(movieTheaters);
            var genresVM = _mapper.Map<List<GenreVM>>(genres);
            return new MoviePostGetVM() { Genres = genresVM, MovieTheaters = movieTheatersVM };
        }
        [HttpPost]
        public async Task<ActionResult<int>> Add([FromForm] MovieCreateVM model)
        {
            var movie = _mapper.Map<Movie>(model);
            if (model.Poster != null)
            {
                movie.Poster = await _fileStorageService.SaveFile(containerName, model.Poster);
            }
            AnnotateActorsOrder(movie);
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie.Id);

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id) {
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
                return NotFound();

            _context.Remove(movie);
            await _context.SaveChangesAsync();
            await _fileStorageService.DeleteFile(movie.Poster, containerName);
            return NoContent();

        }
        private void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }
    }
}
