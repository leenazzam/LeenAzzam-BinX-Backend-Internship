namespace WebApplication1.models
{
    public class TaskAssignment
    {
        public int Id { get; set; }


        // Foreign Key for Task
        public int TaskId { get; set; }

        public AppTask Task { get; set; }


        // Foreign Key for User
        public int UserId { get; set; }

        public User User { get; set; }
    }
}