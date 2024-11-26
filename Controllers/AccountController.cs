using AutoMapper;
using AutoMapper.QueryableExtensions;
using EHospital.DTO;
using EHospital.Models;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        TokenService tokenService,
        HospitalDbContext context,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly TokenService _tokenService = tokenService;
        private readonly HospitalDbContext _context = context;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginModel loginModel)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == loginModel.NameIdentifier || u.UserName == loginModel.NameIdentifier ||
                u.PhoneNumber == loginModel.NameIdentifier);
            if (user == null)
            {
                return Unauthorized(new { email = "User not found." });
            }

            var result =
                _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginModel.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { password = "Invalid password." });
            }

            // generate token
            var token = await _tokenService.GenerateToken(user, null);

            return Ok(new LoginResponse
            {
                Token = token,
                User = _mapper.Map<UserProfileDTO>(user)
            });
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpGet("@Me")]
        public async Task<ActionResult<UserProfileDTO>> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            var patient = _context.Patients.FirstOrDefault(p => p.UserId == user.Id);
            var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == user.Id);
            UserProfileDTO userProfile = _mapper.Map<UserProfileDTO>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userProfile.Roles = roles;
            userProfile.Patient = _mapper.Map<PatientDTO>(patient);
            userProfile.Doctor = _mapper.Map<DoctorDTO>(doctor);
            return Ok(userProfile);
        }

        [HttpGet("login/google")]
        public IActionResult LoginWithGoogle()
        {
            var protocol = Request.IsHttps ? "https" : "http";
            var feHost = Request.Headers.Origin.FirstOrDefault() ?? Request.Host.ToString();
            var redirectUri = $"{protocol}://{feHost}/auth/google-response";
            var properties = new AuthenticationProperties { RedirectUri = redirectUri };
            // flow react gọi window.location.href = "/api/account/google-login"
            // server trả về trang login google
            // sau khi login xong, google sẽ redirect về React app
            // React app sẽ gọi /api/account/google-response kèm cookie 'Identity.External' để lấy token
            // server sẽ trả về token cho React app
            // var properties = new AuthenticationProperties { RedirectUri = "/api/account/google-response" };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<ActionResult<LoginResponse>> GoogleResponse()
        {
            var result = await this.HttpContext.AuthenticateAsync("Identity.External");
            var principal = result.Principal;
            if (principal == null)
            {
                return Unauthorized();
            }


            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var nameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fullName = principal.FindFirst(ClaimTypes.Name)?.Value;
            // toDo: kiểm tra email có tồn tại trong db chưa, nếu chưa thì tạo mới user
            var existUser = await _userManager.FindByEmailAsync(email!);
            if (existUser == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    UserName = nameIdentifier,
                    NormalizedUserName = nameIdentifier!,
                    EmailConfirmed = true,
                };
                var resultCreate = await _userManager.CreateAsync(user, "Paitent@123");
                await _userManager.AddToRoleAsync(user, "Patient");
                if (!resultCreate.Succeeded)
                {
                    return BadRequest(resultCreate.Errors);
                }

                existUser = user;
                var patient = new Patient
                {
                    Email = email,
                    Name = fullName!,
                    UserId = user.Id
                };
                await _context.Patients.AddAsync(patient);
            }

            var token = await _tokenService.GenerateToken(existUser, null);
            var profileDto = _mapper.Map<UserProfileDTO>(existUser);
            profileDto.Patient = _context.Patients.Where(p => p.UserId == existUser.Id)
                .ProjectTo<PatientDTO>(_mapper.ConfigurationProvider).FirstOrDefault();
            return Ok(new LoginResponse()
            {
                Token = token,
                User = profileDto
            });
        }

        [HttpGet("Identity-Exists")]
        public async Task<ActionResult<bool>> IdentityExists([FromQuery] string identity, [FromQuery] IdentityType type)
        {
            IdentityUser? user = null;
            if (type == IdentityType.Email)
            {
                user = await _userManager.FindByEmailAsync(identity);
            }

            if (type == IdentityType.Phone)
            {
                user = _context.Users.Where(u => u.PhoneNumber == identity).FirstOrDefault();
            }

            if (type == IdentityType.UserName)
            {
                user = await _userManager.FindByNameAsync(identity);
            }

            return Ok(user != null);
        }
    }

    public enum IdentityType
    {
        Email,
        Phone,
        UserName
    }
}