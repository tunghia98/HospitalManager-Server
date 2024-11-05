namespace EHospital.DTO
{
    public class PatientRegistration
    {
        public string Name { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
        public string? Gender { get; set; }
        public string? HealthInsurance { get; set; }
    }
}
