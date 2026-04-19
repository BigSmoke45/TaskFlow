namespace WpfTaskFlow.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string Priority { get; set; } = "";
        public string StatusText { get; set; } = "";
        public string CreatedAtText { get; set; } = "";
        public bool IsCompleted { get; set; }
    }
}