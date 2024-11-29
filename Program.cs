using EHospital.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using EHospital.Services;
using HospitalManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder
    .WithOrigins("http://localhost:8080","http://localhost:80","http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
});
builder.Services.AddSwaggerGen(option =>
{
    // only action name
    option.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Hopital Management System Api", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddDbContext<HospitalDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("cnn")));

builder.Services.AddAuthorization();
builder.Services.AddTransient<IPasswordHasher<IdentityUser>, BCryptPasswordHasher<IdentityUser>>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddPasswordValidator<PasswordValidator<IdentityUser>>()
                    .AddEntityFrameworkStores<HospitalDbContext>()
                    .AddDefaultTokenProviders()
                    ;
builder.Services.AddAppServices();
builder.Services.AddSignalR();
var configuration = builder.Configuration;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// .AddCookie("Cookies")
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
    };
}).AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
        googleOptions.CallbackPath = "/signin-google";
        googleOptions.SignInScheme = "Identity.External";
        googleOptions.Scope.Add("profile");
        googleOptions.Scope.Add("email");
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.MapHub<MessageService>("/api/realtime");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
HospitalDbContext.SeedRolesAsync(app.Services).Wait();
app.Run();
