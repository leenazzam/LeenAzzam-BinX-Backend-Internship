namespace WebApplication1.DTOs
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public int ProjectId { get; set; }
    }
}