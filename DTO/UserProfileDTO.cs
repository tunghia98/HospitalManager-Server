using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace EHospital.DTO
{
    public class UserProfileDTO
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public string? Address { get; set; }
        public IEnumerable<string>? Roles { get; set; }
        public PatientDTO? Patient { get; set; }
        public DoctorDTO? Doctor { get; set; }
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<IdentityUser, UserProfileDTO>();
            }
        }
    }


}