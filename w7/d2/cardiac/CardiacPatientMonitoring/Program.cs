using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CardiacPatientMonitoring.Data;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.Repositories;
using CardiacPatientMonitoring.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Controllers
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your JWT token"
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddOpenApi();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Identity
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };
    });

// Authorization
builder.Services.AddAuthorization();

// Vital sign repository/service (business logic: critical-value detection)
builder.Services.AddScoped<IVitalSignRepository, VitalSignRepository>();
builder.Services.AddScoped<VitalSignService>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;

    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("GeneralPolicy", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();


// =====================================================
// DATABASE SEEDING
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityUser>>();

    var context =
        services.GetRequiredService<AppDbContext>();


    // =================================================
    // CREATE ROLES
    // =================================================

    string[] roles =
    {
        "Admin",
        "Doctor",
        "Patient"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role)
            );
        }
    }


    // =================================================
    // CREATE DEFAULT ADMIN
    // =================================================

    var adminEmail = "admin@cardiac.com";
    var adminPassword = "Admin123!";

    var adminUser =
        await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result =
            await userManager.CreateAsync(
                adminUser,
                adminPassword
            );

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin"
            );
        }
    }
    else
    {
        // Make sure existing admin has Admin role
        if (!await userManager.IsInRoleAsync(
                adminUser,
                "Admin"))
        {
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin"
            );
        }
    }


    // =================================================
    // CREATE PATIENT USERS + PATIENTS
    // =================================================

    if (!context.Patients.Any())
    {
        // ---------------------------------------------
        // Patient User 1
        // ---------------------------------------------

        var patientUser1 =
            await userManager.FindByEmailAsync(
                "ahmad@cardiac.com"
            );

        if (patientUser1 == null)
        {
            patientUser1 = new IdentityUser
            {
                UserName = "ahmad@cardiac.com",
                Email = "ahmad@cardiac.com",
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    patientUser1,
                    "Patient123!"
                );

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    patientUser1,
                    "Patient"
                );
            }
            else
            {
                throw new Exception(
                    "Failed to create Ahmad patient user: " +
                    string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description)
                    )
                );
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(
                    patientUser1,
                    "Patient"))
            {
                await userManager.AddToRoleAsync(
                    patientUser1,
                    "Patient"
                );
            }
        }


        // ---------------------------------------------
        // Patient User 2
        // ---------------------------------------------

        var patientUser2 =
            await userManager.FindByEmailAsync(
                "sara@cardiac.com"
            );

        if (patientUser2 == null)
        {
            patientUser2 = new IdentityUser
            {
                UserName = "sara@cardiac.com",
                Email = "sara@cardiac.com",
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    patientUser2,
                    "Patient123!"
                );

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    patientUser2,
                    "Patient"
                );
            }
            else
            {
                throw new Exception(
                    "Failed to create Sara patient user: " +
                    string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description)
                    )
                );
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(
                    patientUser2,
                    "Patient"))
            {
                await userManager.AddToRoleAsync(
                    patientUser2,
                    "Patient"
                );
            }
        }


        // =================================================
        // CREATE PATIENT RECORDS
        // =================================================

        var patient1 = new Patient
        {
            FullName = "Ahmad Youssef",
            Age = 55,
            Gender = "Male",
            PhoneNumber = "0599111222",

            // IMPORTANT:
            // Connect Patient with Identity User
            IdentityUserId = patientUser1.Id
        };

        var patient2 = new Patient
        {
            FullName = "Sara Khalil",
            Age = 62,
            Gender = "Female",
            PhoneNumber = "0599333444",

            // IMPORTANT:
            // Connect Patient with Identity User
            IdentityUserId = patientUser2.Id
        };

        context.Patients.AddRange(
            patient1,
            patient2
        );

        context.SaveChanges();


        // =================================================
        // VITAL SIGNS
        // =================================================

        context.VitalSigns.AddRange(

            new VitalSign
            {
                PatientId = patient1.Id,
                HeartRate = 78,
                BloodPressure = "120/80",
                OxygenLevel = 97,
                RecordedAt = DateTime.UtcNow
            },

            new VitalSign
            {
                PatientId = patient2.Id,
                HeartRate = 85,
                BloodPressure = "130/85",
                OxygenLevel = 95,
                RecordedAt = DateTime.UtcNow
            }
        );


        // =================================================
        // MEDICATIONS
        // =================================================

        context.Medications.AddRange(

            new Medication
            {
                PatientId = patient1.Id,
                Name = "Aspirin",
                Dosage = "100mg",
                Frequency = "Once daily"
            },

            new Medication
            {
                PatientId = patient2.Id,
                Name = "Metoprolol",
                Dosage = "50mg",
                Frequency = "Twice daily"
            }
        );


        // =================================================
        // APPOINTMENTS
        // =================================================

        context.Appointments.AddRange(

            new Appointment
            {
                PatientId = patient1.Id,
                AppointmentDate =
                    DateTime.UtcNow.AddDays(7),
                DoctorName = "Dr. Layla Hassan",
                Notes = "Follow up checkup"
            },

            new Appointment
            {
                PatientId = patient2.Id,
                AppointmentDate =
                    DateTime.UtcNow.AddDays(10),
                DoctorName = "Dr. Omar Nasser",
                Notes = "Routine cardiac evaluation"
            }
        );


        // Save VitalSigns, Medications, Appointments
        context.SaveChanges();
    }
}


// =====================================================
// HTTP PIPELINE
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}


// =====================================================
// GLOBAL EXCEPTION HANDLING
// =====================================================

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;

        context.Response.ContentType =
            "application/problem+json";

        var logger =
            context.RequestServices
                .GetRequiredService<ILogger<Program>>();

        var feature =
            context.Features.Get<
                Microsoft.AspNetCore.Diagnostics
                    .IExceptionHandlerFeature>();

        logger.LogError(
            feature?.Error,
            "Unhandled exception occurred for request {Path}",
            context.Request.Path
        );

        await context.Response.WriteAsJsonAsync(
            new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = 500
            }
        );
    });
});


app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Required for integration tests
public partial class Program
{
}