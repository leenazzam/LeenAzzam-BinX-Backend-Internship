using Moq;
using WebApplication1.models;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1.Tests
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task IsTaskOverdueAsync_ReturnsTrue_WhenTaskIsOverdueAndNotCompleted()
        {
            var mockRepo = new Mock<ITaskRepository>();

            mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new AppTask
                {
                    Id = 1,
                    Title = "Test Task",
                    Status = "InProgress",
                    DueDate = new DateTime(2026, 1, 1),
                    ProjectId = 1
                });

            var checker = new TaskStatusChecker();
            var service = new TaskService(mockRepo.Object, checker);

            var result = await service.IsTaskOverdueAsync(1, new DateTime(2026, 8, 16));

            Assert.True(result);
        }
        [Fact]
public async Task IsTaskOverdueAsync_ThrowsException_WhenRepositoryFails()
{
    var mockRepo = new Mock<ITaskRepository>();

    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ThrowsAsync(new InvalidOperationException("Database connection failed"));

    var checker = new TaskStatusChecker();
    var service = new TaskService(mockRepo.Object, checker);

    await Assert.ThrowsAsync<InvalidOperationException>(
        () => service.IsTaskOverdueAsync(1, new DateTime(2026, 8, 16))
    );
}

[Fact]
public async Task IsTaskOverdueAsync_CallsGetByIdAsyncExactlyOnce()
{
    var mockRepo = new Mock<ITaskRepository>();

    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new AppTask
        {
            Id = 1,
            Title = "Test Task",
            Status = "In Progress",
            DueDate = new DateTime(2026, 1, 1),
            ProjectId = 1
        });

    var checker = new TaskStatusChecker();
    var service = new TaskService(mockRepo.Object, checker);

    await service.IsTaskOverdueAsync(1, new DateTime(2026, 8, 16));

    mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
}
    }
}
