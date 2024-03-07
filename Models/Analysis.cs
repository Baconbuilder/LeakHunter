using System;
using System.Collections.Generic;

namespace LeakHunter.Models
{
    public partial class Analysis
    {
        public int AId { get; set; }
        public int? CId { get; set; }
        public int? TId { get; set; }
        public string? Content { get; set; }
        public int? Count { get; set; }
    }
}
