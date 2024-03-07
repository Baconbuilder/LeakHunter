using System;
using System.Collections.Generic;

namespace LeakHunter.Models
{
    public partial class PostAnalysis
    {
        public int AId { get; set; }
        public int? CId { get; set; }
        public int? TId { get; set; }
        public string? PreContent { get; set; }
        public string? PostContent { get; set; }
        public DateTime? Time { get; set; }
        public string? Url { get; set; }
        public string? WebName { get; set; }
        public int? Count { get; set; }
    }
}
