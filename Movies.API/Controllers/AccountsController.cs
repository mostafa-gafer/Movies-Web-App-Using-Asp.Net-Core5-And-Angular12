using AutoMapper;
using Helper.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Movies.CORE.Context;
using Movies.CORE.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public AccountsController(UserManager<IdentityUser> userManger, 
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _userManger = userManger;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }
        [HttpGet("listUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<List<UserVM>> GetListUsers([FromQuery] PaginationVM paginationVM)
        {
            var queryable = _context.Users.AsQueryable();
            await HttpContext.InsertParamtersPaginationInHeader(queryable);
            var users = await queryable.OrderBy(x => x.Email).paginate(paginationVM).ToListAsync();
            return _mapper.Map<List<UserVM>>(users);
        }

        [HttpPost("makeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<IActionResult> MakeAdmin([FromBody] string userId)
        {
            var user = await _userManger.FindByIdAsync(userId);
            await _userManger.AddClaimAsync(user, new Claim("role", "admin"));
            return NoContent();
        }
        [HttpPost("removeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<IActionResult> RemoveAdmin([FromBody] string userId)
        {
            var user = await _userManger.FindByIdAsync(userId);
            await _userManger.RemoveClaimAsync(user, new Claim("role", "admin"));
            return NoContent();
        }
        [HttpPost("create")]
        public async Task<ActionResult<AuthenticationResponse>> Create([FromBody] UserCredentials userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await _userManger.CreateAsync(user, userCredentials.Password);
            return (result.Succeeded )? await BuildToken(userCredentials) : BadRequest(result.Errors);


        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] UserCredentials userCredentials)
        {

            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password,
                isPersistent: false, lockoutOnFailure: false);
            return (result.Succeeded) ? await BuildToken(userCredentials) : BadRequest("Incorret Login");


        }
        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };
            var user = await _userManger.FindByNameAsync(userCredentials.Email);   //not that name work also
            var claimsDB = await _userManger.GetClaimsAsync(user);
            claims.AddRange(claimsDB);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);
            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
