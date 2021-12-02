using AutoMapper;
using Helper.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class MovieTheatersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MovieTheatersController> _logger;
        private readonly IMapper _mapper;

        public MovieTheatersController(IUnitOfWork unitOfWork, ILogger<MovieTheatersController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterVM>>> Get()
        {
            var entities = await _unitOfWork.MovieTheaters.GetAllAsync();
           
            return _mapper.Map<List<MovieTheaterVM>>(entities);
        }
        //[HttpGet("{id:int}")]
        [HttpGet("GetById")]
        public async Task<ActionResult<MovieTheaterVM>> GetById(int id)
        {
            var movieTheater = await _unitOfWork.MovieTheaters.GetByIdAsync(id);
            if (movieTheater == null)
                return NotFound();

            return _mapper.Map<MovieTheaterVM>(movieTheater);
        }
        [HttpPost]
        public async Task<ActionResult<MovieTheaterVM>> Add([FromBody] MovieTheaterVM model)
        {
            await _unitOfWork.MovieTheaters.AddAsync(_mapper.Map<MovieTheater>(model));
             _unitOfWork.Complete();
            return NoContent();
        }
        [HttpPut]
        public async Task<ActionResult<MovieTheaterVM>> Put([FromBody]MovieTheaterVM model)
        {
             _unitOfWork.MovieTheaters.UpdateAsync(_mapper.Map<MovieTheater>(model));
            _unitOfWork.Complete();
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            var entity = await _unitOfWork.MovieTheaters.GetByIdAsync(Id);
            if (entity == null)
                return NotFound();

            _unitOfWork.MovieTheaters.Delete(entity);
            _unitOfWork.Complete();
            return NoContent();
        }
    }
}
