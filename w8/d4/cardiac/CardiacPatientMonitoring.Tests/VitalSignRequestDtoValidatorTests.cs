using FluentValidation.Results;
using CardiacPatientMonitoring.DTOs;
using CardiacPatientMonitoring.Validators;
using Xunit;

namespace CardiacPatientMonitoring.Tests;

public class VitalSignRequestDtoValidatorTests
{
    private readonly VitalSignRequestDtoValidator _validator = new VitalSignRequestDtoValidator();

    [Fact]
    public void Task1_ValidVitalSign_ShouldPass()
    {
        var dto = new VitalSignRequestDto
        {
            PatientId = 1,
            HeartRate = 80,
            BloodPressure = "120/80",
            OxygenLevel = 98,
            RecordedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        ValidationResult result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Task2_HeartRateTooHigh_ShouldFail()
    {
        var dto = new VitalSignRequestDto
        {
            PatientId = 1,
            HeartRate = 300,
            BloodPressure = "120/80",
            OxygenLevel = 98,
            RecordedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        ValidationResult result = _validator.Validate(dto);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Task3_HeartRateTooLow_ShouldFail()
    {
        var dto = new VitalSignRequestDto
        {
            PatientId = 1,
            HeartRate = 10,
            BloodPressure = "120/80",
            OxygenLevel = 98,
            RecordedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        ValidationResult result = _validator.Validate(dto);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Task4_OxygenLevelTooLow_ShouldFail()
    {
        var dto = new VitalSignRequestDto
        {
            PatientId = 1,
            HeartRate = 80,
            BloodPressure = "120/80",
            OxygenLevel = 30,
            RecordedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        ValidationResult result = _validator.Validate(dto);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Task5_InvalidBloodPressureFormat_ShouldFail()
    {
        var dto = new VitalSignRequestDto
        {
            PatientId = 1,
            HeartRate = 80,
            BloodPressure = "abc",
            OxygenLevel = 98,
            RecordedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        ValidationResult result = _validator.Validate(dto);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Task6_RecordedAtInFuture_ShouldFail()
    {
        var dto = new VitalSignRequestDto
        {
            PatientId = 1,
            HeartRate = 80,
            BloodPressure = "120/80",
            OxygenLevel = 98,
            RecordedAt = DateTime.UtcNow.AddDays(1)
        };

        ValidationResult result = _validator.Validate(dto);

        Assert.False(result.IsValid);
    }
}
