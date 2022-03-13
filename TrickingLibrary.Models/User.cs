﻿using System.Collections.Generic;

namespace TrickingLibrary.Models
{
    public class User : BaseModel<string>
    {
        public string Name { get; set; }
        
        public string Username { get; set; }

        public IList<Submission> Submissions { get; set; } = new List<Submission>();
    }
}