using AutoMapper;
using EHospital.DTO;
using EHospital.Models;
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
        IMapper mapper) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly TokenService _tokenService = tokenService;
        private readonly HospitalDbContext _context = context;

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginModel loginModel)
        {
            var user =  _context.Users.Where(u => u.Email == loginModel.NameIdentifier || u.UserName == loginModel.NameIdentifier || u.PhoneNumber == loginModel.NameIdentifier).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
           
            if(!result.Succeeded)
            {
                return Unauthorized(result);
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
            var patient = _context.Patients.Where(p => p.UserId == user.Id).FirstOrDefault();
            var doctor = _context.Doctors.Where(d => d.UserId == user.Id).FirstOrDefault();
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
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await this.HttpContext.AuthenticateAsync("Identity.External");
            var principal = result.Principal;
            if (principal == null)
            {
                return Unauthorized();
            }

            var isAuthenticated = principal.Identity!.IsAuthenticated;

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var nameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // toDo: kiểm tra email có tồn tại trong db chưa, nếu chưa thì tạo mới user
            // toDo: tạo token và trả về client
            return Ok(new { name = principal?.Identity?.Name });
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
                user =  _context.Users.Where(u => u.PhoneNumber == identity).FirstOrDefault();
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