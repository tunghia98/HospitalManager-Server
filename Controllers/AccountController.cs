﻿using AutoMapper;
using EHospital.DTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IMapper mapper): ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid email or password."); // More descriptive error
            }
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            // Get roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, loginModel.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id),
            };
            // Add role claims
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString())); // Add each role as a claim
            }

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JWT:Expires"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );

            return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) }); // Return the token in a structured response
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }
        [HttpGet("@Me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine(User.Claims.Count());
            if (user == null)
            {
                return Forbid();
            }
            UserProfileDTO userProfile = _mapper.Map<UserProfileDTO>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userProfile.Roles = roles;

            return Ok(userProfile);
        }
    }
    
}
