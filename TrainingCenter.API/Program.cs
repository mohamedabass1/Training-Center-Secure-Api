using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using TrainingCenter.API.Authorization;
using TrainingCenter.API.Common;
using TrainingCenter.Application.Services;
using TrainingCenter.Application.Services.Auth;
using TrainingCenter.Application.Settings;
using TrainingCenter.Application.Validators.Instructors;
using TrainingCenter.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);


// ==================================================
//                  Configuration
// ==================================================

JwtSettings jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;


// ==================================================
//                     CORS
// ==================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("TrainingCenterApiCrosPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ==================================================
//              Controllers & JSON
// ==================================================

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });


// ==================================================
//               FluentValidation
// ==================================================

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<CreateInstructorDtoValidator>();


// ==================================================
//                    Database
// ==================================================

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


// ==================================================
//                 JWT Settings
// ==================================================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));


// ==================================================
//              Authentication
// ==================================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
    });



// ==================================================
//                Authorization
// ==================================================



builder.Services.AddScoped<IAuthorizationHandler, StudentOwnerOrAdminHandler>();

builder.Services.AddScoped<IAuthorizationHandler, InstructorOwnerOrAdminHandler>();

builder.Services.AddScoped<IAuthorizationHandler, CourseOwnerOrAdminHandler>();

builder.Services.AddScoped<IAuthorizationHandler, EnrollmentOwnerOrAdminHandler>();

builder.Services.AddScoped<IAuthorizationHandler, EnrollmentCourseInstructorOrAdminHandler>();

// This enables attributes like [Authorize] and role-based authorization.


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOwnerOrAdmin", policy =>
            policy.Requirements.Add(new StudentOwnerOrAdminRequirement()));


    options.AddPolicy("InstructorOwnerOrAdmin", policy =>
            policy.Requirements.Add(new InstructorOwnerOrAdminRequirement()));

    options.AddPolicy("CourseOwnerOrAdmin", policy =>
            policy.Requirements.Add(new CourseOwnerOrAdminRequirement()));

    options.AddPolicy("EnrollmentOwnerOrAdmin", policy =>
            policy.Requirements.Add(new EnrollmentOwnerOrAdminRequirement()));

    options.AddPolicy("EnrollmentCourseInstructorOrAdmin", policy =>
            policy.Requirements.Add(new EnrollmentCourseInstructorOrAdminRequirement()));

});

// ==================================================
//                     Swagger
// ==================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter: Bearer {your JWT token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});



// ==================================================
//             Application Services
// ==================================================

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<InstructorService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();


var app = builder.Build();


// ==================================================
//               Global Middleware
// ==================================================

app.UseMiddleware<GlobalExceptionMiddleware>();


// ==================================================
//             HTTP Request Pipeline
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("TrainingCenterApiCrosPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();