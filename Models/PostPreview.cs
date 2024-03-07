using System;
using System.Collections.Generic;

namespace LeakHunter.Models
{
    public partial class PostPreview
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? Time { get; set; }
        public string? Url { get; set; }
        public string? WebName { get; set; }
    }
}
