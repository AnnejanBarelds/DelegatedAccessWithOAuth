namespace ApplicationAPI
{
    public class Todo
    {
        public int Id { get; set; }

        public string Task { get; set; }

        public DateTimeOffset DueDate { get; set; }

        public bool Completed { get; set; }
    }
}
