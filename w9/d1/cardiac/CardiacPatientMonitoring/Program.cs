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
using CardiacPatientMonitoring.Middleware;

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
    )
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging());



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

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});


var app = builder.Build();
app.UseMiddleware<RequestTimingMiddleware>();

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
        // =================================================
// EXTRA SEED DATA FOR LOAD TESTING (N+1 demo)
// =================================================

var random = new Random();

for (int i = 1; i <= 18; i++)
{
    var extraUser = new IdentityUser
    {
        UserName = $"patient{i}@cardiac.com",
        Email = $"patient{i}@cardiac.com",
        EmailConfirmed = true
    };

    var createResult = userManager.CreateAsync(extraUser, "Patient123!").Result;

    if (createResult.Succeeded)
    {
        userManager.AddToRoleAsync(extraUser, "Patient").Wait();

        var extraPatient = new Patient
        {
            FullName = $"Test Patient {i}",
            Age = random.Next(30, 80),
            Gender = i % 2 == 0 ? "Female" : "Male",
            PhoneNumber = $"05991{i:D5}",
            IdentityUserId = extraUser.Id
        };

        context.Patients.Add(extraPatient);
        context.SaveChanges();

        var vitalCount = random.Next(2, 5);
        for (int j = 1; j <= vitalCount; j++)
        {
            var isCritical = random.Next(0, 4) == 0;

            var vital = new VitalSign
            {
                PatientId = extraPatient.Id,
                HeartRate = isCritical ? random.Next(160, 190) : random.Next(65, 100),
                BloodPressure = "125/82",
                OxygenLevel = isCritical ? random.Next(80, 89) : random.Next(94, 99),
                RecordedAt = DateTime.UtcNow.AddDays(-random.Next(1, 60))
            };

            context.VitalSigns.Add(vital);
            context.SaveChanges();

            if (isCritical)
            {
                context.Alerts.Add(new Alert
                {
                    PatientId = extraPatient.Id,
                    VitalSignId = vital.Id,
                    Message = $"Critical reading recorded: HR={vital.HeartRate}, SpO2={vital.OxygenLevel}%.",
                    Severity = "Critical",
                    CreatedAt = vital.RecordedAt
                });
            }
        }

        context.SaveChanges();
    }
}
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