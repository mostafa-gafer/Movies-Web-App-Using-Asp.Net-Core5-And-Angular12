using AutoMapper;
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
    public class GenresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GenresController> _logger;
        private readonly IMapper _mapper;
        public GenresController(IUnitOfWork unitOfWork, ILogger<GenresController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GenreVM>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all the genres");
                IEnumerable<Genre> genresList = await _unitOfWork.Genres.GetAllAsync();
                var genres = genresList.OrderBy(o => o.Name);
                return Ok(_mapper.Map<IEnumerable<GenreVM>>(genres));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        [HttpGet("GetById")]
        public async Task<ActionResult<GenreVM>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting genre By Id");
                var genres = await _unitOfWork.Genres.GetByIdAsync(id);
                
                return Ok(_mapper.Map<GenreVM>(genres));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GenreVM model)
        {
            try
            {
                var genre = _mapper.Map<Genre>(model);
                await _unitOfWork.Genres.AddAsync(genre);
                _unitOfWork.Complete();
                return Ok();
            } catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        // [HttpDelete("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                Genre entity = await _unitOfWork.Genres.GetByIdAsync(Id);
                _unitOfWork.Genres.Delete(entity);
                _unitOfWork.Complete(); 
                return Ok();

            }catch(Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        [HttpPut("{id = 0}")]
        public IActionResult Put(int id,[FromBody]GenreVM entity)
        {
            try
            {
                 _unitOfWork.Genres.UpdateAsync(_mapper.Map<Genre>(entity));
                _unitOfWork.Complete();
                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
    }
}
