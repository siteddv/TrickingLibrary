﻿using System;
using System.Collections.Generic;
using TrickingLibrary.Models.Abstractions;

namespace TrickingLibrary.Models
{
    public class Submission : BaseModel<int>
    {
        public string TrickId { get; set; }
        public Video Video { get; set; }
        public bool VideoProcessed { get; set; }
        public string Description { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public IList<SubmissionVote> Votes { get; set; } = new List<SubmissionVote>();
        public IList<Comment> Comments { get; set; } = new List<Comment>();
    }
}