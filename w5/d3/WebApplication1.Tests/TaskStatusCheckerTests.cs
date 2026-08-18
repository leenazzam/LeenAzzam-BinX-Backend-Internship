using WebApplication1.Services;

namespace WebApplication1.Tests
{
    public class TaskStatusCheckerTests
    {
        // Task 1
        [Fact]
        public void IsOverdue_ReturnsTrue_WhenDueDatePassedAndNotCompleted()
        {
            var checker = new TaskStatusChecker();

            var result = checker.IsOverdue(new DateTime(2026, 1, 1), "InProgress", new DateTime(2026, 8, 16));

            Assert.True(result);
        }

        // Task 2
        [Fact]
        public void IsOverdue_ReturnsFalse_WhenDueDateInFuture()
        {
            var checker = new TaskStatusChecker();

            var result = checker.IsOverdue(new DateTime(2027, 1, 1), "InProgress", new DateTime(2026, 8, 16));

            Assert.False(result);
        }

        // Task 3
        [Fact]
        public void IsOverdue_ReturnsFalse_WhenTaskIsCompleted()
        {
            var checker = new TaskStatusChecker();

            var result = checker.IsOverdue(new DateTime(2026, 1, 1), "Completed", new DateTime(2026, 8, 16));

            Assert.False(result);
        }

        // Theory test
        [Theory]
        [InlineData("2026-01-01", "InProgress", "2026-08-16", true)]
        [InlineData("2027-01-01", "InProgress", "2026-08-16", false)]
        [InlineData("2026-01-01", "Completed", "2026-08-16", false)]
        public void IsOverdue_HandlesMultipleCases(string dueDate, string status, string currentDate, bool expected)
        {
            var checker = new TaskStatusChecker();

            var result = checker.IsOverdue(DateTime.Parse(dueDate), status, DateTime.Parse(currentDate));

            Assert.Equal(expected, result);
        }
    }
}