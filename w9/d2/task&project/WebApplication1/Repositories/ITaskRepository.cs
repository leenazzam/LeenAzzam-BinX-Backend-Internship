using WebApplication1.models;

namespace WebApplication1.Repositories
{
    public interface ITaskRepository
    {
        Task<AppTask?> GetByIdAsync(int id);
    }
}
