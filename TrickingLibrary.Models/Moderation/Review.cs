﻿namespace TrickingLibrary.Models.Moderation
{
    public class Review : BaseModel<int>
    {
        public int ModerationItemId { get; set; }
        public ModerationItem ModerationItem { get; set; }

        public string Comment { get; set; }
        public ReviewStatus Status { get; set; }
    }
}