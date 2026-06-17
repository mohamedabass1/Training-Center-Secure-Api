using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using TrainingCenter.API.Common;
using TrainingCenter.Application.Services;
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

// This enables attributes like [Authorize] and role-based authorization.
builder.Services.AddAuthorization();


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

app.UseRouting();


app.UseCors("TrainingCenterApiCrosPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();