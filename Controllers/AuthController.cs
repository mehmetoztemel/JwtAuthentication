using JwtAuthentication.Context;
using JwtAuthentication.Dtos;
using JwtAuthentication.Models;
using JwtAuthentication.Services;
using JwtAuthentication.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MO.Mapper;
using MO.Result;

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        ApplicationDbContext _dbContext;
        private readonly IJwtProvider _jwtProvider;
        public AuthController(ApplicationDbContext dbContext, IJwtProvider jwtProvider)
        {
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(CreateAppUserDto request, CancellationToken cancellationToken = default)
        {
            if (_dbContext.AppUsers.Any(x => x.Email == request.Email))
            {
                return BadRequest(Result<string>.Failure("This e-mail address has been used before."));
            }

            var user = Mapper.Map<CreateAppUserDto, AppUser>(request);
            byte[] passwordHash, passwordSalt;
            PasswordHelper.CreatePassword(request.Password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _dbContext.AppUsers.AddAsync(user, cancellationToken);
            int status = await _dbContext.SaveChangesAsync(cancellationToken);

            if (status > 0)
            {
                return Ok(Result<string>.Success("User registered."));
            }
            else
            {
                return BadRequest(Result<string>.Failure("User registration failed."));
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto request, CancellationToken cancellationToken = default)
        {
            AppUser? user = await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Email == request.Email, default);
            if (user is null)
            {
                return NotFound(Result<string>.Failure("User can not find"));
            }

            if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest(Result<string>.Failure("Password is wrong"));
            }

            return Ok(Result<string>.Success(_jwtProvider.CreateToken(user)));
        }
    }
}