using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace file_box.Models
{
    // the database model
    public partial class File
    {        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public int CreatedAt { get; set; }
    }
}
