namespace EHospital.Services;

using Microsoft.AspNetCore.Identity;
using BCrypt.Net;

public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    public string HashPassword(TUser user, string password)
    {
        return BCrypt.HashPassword(password);
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        var isPasswordValid = BCrypt.Verify(providedPassword, hashedPassword);

        return isPasswordValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}