namespace EHospital.DTO
{
    public class DoctorRegistration
    {
        public string Name { get; set; } = null!;
        public string? Specialization { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
        public int? DepartmentId { get; set; }
    }

}
