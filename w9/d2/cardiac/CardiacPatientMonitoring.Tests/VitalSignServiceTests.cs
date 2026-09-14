using Moq;
using CardiacPatientMonitoring.Models;
using CardiacPatientMonitoring.Repositories;
using CardiacPatientMonitoring.Services;
using Xunit;

namespace CardiacPatientMonitoring.Tests;

public class VitalSignServiceTests
{
    [Fact]
    public void Task1_HighHeartRate_ShouldBeCritical()
    {
        var mockRepo = new Mock<IVitalSignRepository>();
        var service = new VitalSignService(mockRepo.Object);

        var vitalSign = new VitalSign
        {
            HeartRate = 180,
            OxygenLevel = 97
        };

        bool result = service.IsCritical(vitalSign);

        Assert.True(result);
    }

    [Fact]
    public void Task2_LowHeartRate_ShouldBeCritical()
    {
        var mockRepo = new Mock<IVitalSignRepository>();
        var service = new VitalSignService(mockRepo.Object);

        var vitalSign = new VitalSign
        {
            HeartRate = 30,
            OxygenLevel = 97
        };

        bool result = service.IsCritical(vitalSign);

        Assert.True(result);
    }

    [Fact]
    public void Task3_LowOxygenLevel_ShouldBeCritical()
    {
        var mockRepo = new Mock<IVitalSignRepository>();
        var service = new VitalSignService(mockRepo.Object);

        var vitalSign = new VitalSign
        {
            HeartRate = 80,
            OxygenLevel = 85
        };

        bool result = service.IsCritical(vitalSign);

        Assert.True(result);
    }

    [Fact]
    public void Task4_NormalReading_ShouldNotBeCritical()
    {
        var mockRepo = new Mock<IVitalSignRepository>();
        var service = new VitalSignService(mockRepo.Object);

        var vitalSign = new VitalSign
        {
            HeartRate = 80,
            OxygenLevel = 97
        };

        bool result = service.IsCritical(vitalSign);

        Assert.False(result);
    }

    [Fact]
    public async Task Task5_AddAsync_ShouldCallRepositoryOnce()
    {
        var mockRepo = new Mock<IVitalSignRepository>();
        var service = new VitalSignService(mockRepo.Object);

        var vitalSign = new VitalSign
        {
            HeartRate = 80,
            OxygenLevel = 97
        };

        await service.AddAsync(vitalSign);

        mockRepo.Verify(r => r.AddAsync(vitalSign), Times.Once);
    }
}
