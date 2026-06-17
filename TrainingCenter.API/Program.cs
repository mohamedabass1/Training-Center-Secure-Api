using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TrainingCenter.API.Common;
using TrainingCenter.Application.Services;
using TrainingCenter.Application.Validators.Instructors;
using TrainingCenter.Infrastructure.Context;


var builder = WebApplication.CreateBuilder(args);

// CROS Conflagration
builder.Services.AddCors(options =>
{
    options.AddPolicy("TrainingCenterApiCrosPolicy", policy =>
    {
        policy
            .WithOrigins(
                 "http://localhost:5173",
            "https://localhost:5173"

            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==================================================
//                Controllers & JSON
// ==================================================

// Register API controllers
// Configure enums to be returned as strings instead of numbers
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });


// ==================================================
//              FluentValidation
// ==================================================

// Enable automatic model validation
builder.Services.AddFluentValidationAutoValidation();

// Register all validators from the Application assembly
builder.Services
    .AddValidatorsFromAssemblyContaining<CreateInstructorDtoValidator>();


// ==================================================
//                   Swagger
// ==================================================

// Generate OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ==================================================
//                   Database
// ==================================================

// Register EF Core DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


// ==================================================
//                Application Services
// ==================================================

// Register business services for Dependency Injection
builder.Services.AddScoped<InstructorService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();


var app = builder.Build();

// Handle all exceptions globally
app.UseMiddleware<GlobalExceptionMiddleware>();


// ==================================================
//              HTTP Request Pipeline
// ==================================================

// Enable Swagger UI only in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

app.UseRouting();

// CORS
app.UseCors("TrainingCenterApiCrosPolicy");

// Enable authorization middle-ware
app.UseAuthorization();


// Map controller endpoints
app.MapControllers();


// Start the application
app.Run();