namespace EHospital.DTO
{
    public class TokenDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
    public class LoginResponse
    {
        public TokenDTO Token { get; set; } = null!;
        public UserProfileDTO User { get; set; } = null!;
    }
}