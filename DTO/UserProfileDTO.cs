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
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public IEnumerable<string>? Roles { get; set; }

        


        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<IdentityUser, UserProfileDTO>();
            }
        }
    }


}