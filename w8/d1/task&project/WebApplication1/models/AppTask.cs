namespace WebApplication1.models
{
    public class AppTask
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public DateTime DueDate { get; set; }


        // Foreign Key
        public int ProjectId { get; set; }

        // Navigation Property
        public Project? Project { get; set; }
    }
}
