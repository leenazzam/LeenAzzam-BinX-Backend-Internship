namespace WebApplication1.Services
{
    public class TaskStatusChecker
    {
        public bool IsOverdue(DateTime dueDate, string status, DateTime currentDate)
        {
            return dueDate < currentDate && status != "Completed";
        }
    }
}