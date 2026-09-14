using WebApplication1.models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _repository;
        private readonly TaskStatusChecker _checker;

        public TaskService(ITaskRepository repository, TaskStatusChecker checker)
        {
            _repository = repository;
            _checker = checker;
        }

        public async Task<bool> IsTaskOverdueAsync(int taskId, DateTime currentDate)
        {
            var task = await _repository.GetByIdAsync(taskId);

            if (task == null)
                return false;

            return _checker.IsOverdue(task.DueDate, task.Status, currentDate);
        }
    }
}
