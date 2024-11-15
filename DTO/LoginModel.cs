using System.ComponentModel.DataAnnotations;

namespace EHospital.DTO
{
    public class LoginModel
    {
        [Required]
        public string NameIdentifier { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }

}
