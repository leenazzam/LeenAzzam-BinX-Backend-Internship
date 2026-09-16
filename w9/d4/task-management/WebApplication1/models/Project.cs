namespace WebApplication1.models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        // Foreign Key for Owner (AspNetUsers)
        public string OwnerId { get; set; }
    }
}