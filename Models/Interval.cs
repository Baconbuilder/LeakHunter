using System;
using System.Collections.Generic;

namespace LeakHunter.Models
{
    public partial class Interval
    {
        public int IId { get; set; }
        public int? UId { get; set; }
        public string? Url { get; set; }
        public string? WebName { get; set; }
        public DateTime? Next { get; set; }
        public int? Day { get; set; }
    }
}
