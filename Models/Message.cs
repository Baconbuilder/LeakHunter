using System;
using System.Collections.Generic;

namespace LeakHunter.Models
{
    public partial class Message
    {
        public int MesId { get; set; }
        public string? Title { get; set; }
        public int? UId { get; set; }
        public string? Content { get; set; }
        public DateTime? Date { get; set; }
    }
}
