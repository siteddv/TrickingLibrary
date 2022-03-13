﻿using TrickingLibrary.Models.Abstractions;

namespace TrickingLibrary.Models
{
    public class Submission : TemporalModel
    {
        public int Id  { get; set; }
        public string TrickId { get; set; }
        public int VideoId { get; set; }
        public Video Video { get; set; }
        public bool VideoProcessed { get; set; }
        public string Description { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }
    }
}