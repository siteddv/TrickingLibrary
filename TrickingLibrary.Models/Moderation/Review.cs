﻿using TrickingLibrary.Models.Abstractions;

namespace TrickingLibrary.Models.Moderation
{
    public class Review : TemporalModel
    {
        public int Id  { get; set; }
        public int ModerationItemId { get; set; }
        public ModerationItem ModerationItem { get; set; }
        public string Comment { get; set; }
        public ReviewStatus Status { get; set; }
    }
}