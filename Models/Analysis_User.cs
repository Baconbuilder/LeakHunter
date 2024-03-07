namespace LeakHunter.Models
{
    public class Analysis_User
    {
        public int UId { get; set; }
        public int AId { get; set; }
        public string? UEmail { get; set; }
        public string? UPassword { get; set; }
        public string? UName { get; set; }
        public int? CId { get; set; }
        public int? TId { get; set; }
        public string? Content { get; set; }
        public string? WContent { get; set; }
        public string? Url { get; set; }
        public string? WebName { get; set; }
        public DateTime? Time { get; set; }
        public int? Count { get; set; }
    }
}
