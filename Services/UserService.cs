
namespace HospitalManagementSystem.Services;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EHospital.DTO;
using EHospital.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

public class UserService(HospitalDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    private readonly HospitalDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    public async Task<IdentityUser> RegisterUser(PatientRegistration registration)
    {

        var user = new IdentityUser
        {
            UserName = Guid.NewGuid().ToString(),
            Email = registration.Email,
            PhoneNumber = registration.PhoneNumber,
            PhoneNumberConfirmed = true,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, registration.Password);
        if (!result.Succeeded)
        {
            throw new BadHttpRequestException(String.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Step 2: Assign the Patient role
        await _userManager.AddToRoleAsync(user, "Patient");

        // Step 3: Create the Patient entry
        var patient = new Models.Patient
        {
            Name = registration.Name,
            DateOfBirth = registration.DateOfBirth,
            PhoneNumber = registration.PhoneNumber,
            Email = registration.Email,
            Gender = registration.Gender,
            HealthInsurance = registration.HealthInsurance,
            UserId = user.Id // Link the patient with the created Identity user
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return user;
    }
}