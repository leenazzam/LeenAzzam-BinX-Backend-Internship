namespace WebApplication1.DTOs
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
    }
}