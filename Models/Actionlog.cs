using System;
using System.Collections.Generic;

namespace file_box.Models
{
    public partial class Actionlog
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string UserRemote { get; set; }
        public int Time { get; set; }
        public string Message { get; set; }
    }
}
