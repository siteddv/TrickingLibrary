﻿using TrickingLibrary.Models.Abstractions;

namespace TrickingLibrary.Models
{
    public class SubmissionVote : Mutable<int>
    {
        public int SubmissionId { get; set; }
        public Submission Submission { get; set; }
        public int Value { get; set; }
    }
}