using AutoMapper;
using Helper.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ActorsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActorsController> _logger;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "actors";

        public ActorsController( IUnitOfWork unitOfWork, ILogger<ActorsController> logger, IMapper mapper, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorVM>>> GetAll([FromQuery] PaginationVM paginationVM)
        {
            try
            {
                //_logger.LogInformation("Getting all the Actors");
                //IEnumerable<Actor> actorsList = await _unitOfWork.Actors.GetAllAsync();
                //var actors = actorsList.OrderBy(o => o.Name);
                var queryable =  _unitOfWork.Actors.GetAllAsQueryable();       // get all as queryable
                await HttpContext.InsertParamtersPaginationInHeader(queryable);//to insert paramters to pagination as header
                var actors = await queryable.OrderBy(o => o.Name).paginate(paginationVM).ToListAsync();//to order actors and paginate rows and send list
                return Ok(_mapper.Map<IEnumerable<ActorVM>>(actors));
                
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        [HttpGet("GetById")]
        public async Task<ActionResult<ActorVM>> GetById(int id)
        {
            try
            {
                
                _logger.LogInformation("Getting Actor By Id");
                var actor = await _unitOfWork.Actors.GetByIdAsync(id);
                return Ok(_mapper.Map<ActorVM>(actor));
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        [HttpPost("searchByName")]
        public async Task<ActionResult<IEnumerable<ActorMovieVM>>> searchByName([FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<ActorMovieVM>();

           var actorsList = await _unitOfWork.Actors.GetAllAsync();
            return actorsList.Where(x => x.Name.Contains(name)).OrderBy(x => x.Name)
                 .Select(x => new ActorMovieVM { Id = x.Id, Name = x.Name, Picture = x.Picture })
                 .Take(5).ToList();                
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ActorCreationVM model)
        {
            try
            {
                _logger.LogInformation("Add Actor");
                var actor = _mapper.Map<Actor>(model);
                if(model.Picture != null)
                {
                    actor.Picture = await _fileStorageService.SaveFile(containerName, model.Picture);
                }
                await _unitOfWork.Actors.AddAsync(actor);
                _unitOfWork.Complete();
                return Ok();
            }catch(Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] ActorCreationVM model)
        {
            try
            {
                _logger.LogInformation("Edit Actor");
                model.Id = id;
                var actor = _mapper.Map<Actor>(model);
                if (model.Picture != null)   //to check if picture changed or not
                    actor.Picture = await _fileStorageService.EditFile(containerName, model.Picture, actor.Picture);
                
                _unitOfWork.Actors.UpdateAsync(actor);
                _unitOfWork.Complete();
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, ex.Message.ToString());
                return StatusCode(500);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                _logger.LogInformation("Delete Actor");
                var entity = await _unitOfWork.Actors.GetByIdAsync(Id);
                _unitOfWork.Actors.Delete(entity);
                _unitOfWork.Complete();
                //await _fileStorageService.DeleteFile(actor.Picture, containerName);
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
