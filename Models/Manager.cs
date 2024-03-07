using System;
using System.Collections.Generic;

namespace LeakHunter.Models
{
    public partial class Manager
    {
        public int MId { get; set; }
        public string? Account { get; set; }
        public string? MPassword { get; set; }
        public string? MName { get; set; }
        public int? State { get; set; }
    }
}
