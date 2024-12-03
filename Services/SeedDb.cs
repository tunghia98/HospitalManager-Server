using EHospital.Models;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace HospitalManagementSystem.Services;
public class SeedDb(HospitalDbContext context, UserManager<IdentityUser> userManager)
{
    private readonly HospitalDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task StartSeed()
    {
        if (_context.Doctors.Any())
        {
            return;
        }

        // Đọc dữ liệu từ tệp JSON
        var data = File.ReadAllText("wwwroot/seed.json");
        var jsonDb = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

        List<Department> departments = JsonConvert.DeserializeObject<List<Department>>(jsonDb["Department"].ToString()) ?? new List<Department>();
        List<Doctor> doctors = JsonConvert.DeserializeObject<List<Doctor>>(jsonDb["Doctor"].ToString()) ?? new List<Doctor>();
        List<Patient> patients = JsonConvert.DeserializeObject<List<Patient>>(jsonDb["Patient"].ToString()) ?? new List<Patient>();
        List<DoctorSchedule> doctorSchedules = JsonConvert.DeserializeObject<List<DoctorSchedule>>(jsonDb["DoctorSchedule"].ToString()) ?? new List<DoctorSchedule>();

        List<IdentityUser> doctorsUser = new List<IdentityUser>();
        List<IdentityUser> patientsUser = new List<IdentityUser>();
        // reset lại các ID
        foreach (var department in departments)
        {
            department.DepartmentId = 0;
        }
        foreach (var doctorSchedule in doctorSchedules)
        {
            doctorSchedule.ScheduleId = 0;
        }
        // reset id của các bảng
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('[dbo].[Department]', RESEED, 1)");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('[dbo].[Doctor]', RESEED, 1)");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('[dbo].[Patient]', RESEED, 1)");
        _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('[dbo].[DoctorSchedule]', RESEED, 1)");

        _context.Departments.AddRange(departments);
        await _context.SaveChangesAsync();

        var random = new Random();

        foreach (var doctor in doctors)
        {
            doctor.DoctorId = 0;
            var width = random.Next(2, 4) * 100;
            var height = width * 1.5;
            doctor.ImageUrl = $"https://picsum.photos/{width}/{height}";
            var identityUser = new IdentityUser
            {
                UserName = doctor.Email,
                Email = doctor.Email,
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                PhoneNumber = doctor.PhoneNumber,
                PhoneNumberConfirmed = true,
            };
            doctor.UserId = identityUser.Id;
            doctorsUser.Add(identityUser);
        }

        foreach (var patient in patients)
        {
            patient.PatientId = 0;
            var identityUser = new IdentityUser
            {
                UserName = patient.Email,
                Email = patient.Email,
                Id = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                PhoneNumber = patient.PhoneNumber,
                PhoneNumberConfirmed = true,
            };
            patient.UserId = identityUser.Id;
            patient.Gender = patient.Gender!.EndsWith('m') ? "MALE" : "FEMALE";
            patientsUser.Add(identityUser); // Thêm vào danh sách patientsUser
        }

        foreach (var doctorUser in doctorsUser)
        {
            await _userManager.CreateAsync(doctorUser, "Sgu@1234");
            await _userManager.AddToRoleAsync(doctorUser, "Doctor");
        }

        foreach (var patientUser in patientsUser)
        {
            await _userManager.CreateAsync(patientUser, "Sgu@1234");
            await _userManager.AddToRoleAsync(patientUser, "Patient");
        }

        await _context.SaveChangesAsync();



        _context.Doctors.AddRange(doctors);
        _context.Patients.AddRange(patients);
        await _context.SaveChangesAsync();


        _context.DoctorSchedules.AddRange(doctorSchedules);
        await _context.SaveChangesAsync();

    }

}