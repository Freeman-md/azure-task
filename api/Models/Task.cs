public class Task {
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime? DueDate { get; set; }

    public List<string> Images { get; set; } = new List<string>();
}