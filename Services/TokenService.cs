namespace HospitalManagementSystem.Services;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EHospital.DTO;
using EHospital.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

public class TokenService(
    HospitalDbContext context,
    IConfiguration configuration,
    RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager)
{
    private readonly HospitalDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    private readonly IConfiguration _configuration = configuration;

    public async Task<TokenDTO> GenerateToken(IdentityUser user, TimeSpan? expiration)
    {
        expiration ??= GetDefaultExpiration();
        var scKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var creds = new SigningCredentials(scKey, SecurityAlgorithms.HmacSha512Signature);
        var roles = await _userManager.GetRolesAsync(user);
        var ownClaims = await _userManager.GetClaimsAsync(user);
        var expirationDate = DateTime.UtcNow.Add(expiration.Value);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Expiration, expirationDate.ToString(), ClaimValueTypes.DateTime)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var claim in ownClaims)
        {
            claims.Add(new Claim(claim.Type, claim.Value));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:ValidIssuer"],
            audience: _configuration["Jwt:ValidAudience"],
            claims: claims,
            expires: expirationDate,
            signingCredentials: creds
        );
        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenDTO
        {
            Token = tokenString,
            Expires = expirationDate
        };
    }

    public TimeSpan GetDefaultExpiration()
    {
        var expiration = _configuration["Jwt:Expires"];
        return TimeSpan.FromMinutes(Convert.ToDouble(expiration));
    }
}