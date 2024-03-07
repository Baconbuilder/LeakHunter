using System;
using System.Collections.Generic;

namespace LeakHunter.Models
{
    public partial class User
    {
        public int UId { get; set; }
        public string? UEmail { get; set; }
        public string? UPassword { get; set; }
        public string? UName { get; set; }
        public string? Permission { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
