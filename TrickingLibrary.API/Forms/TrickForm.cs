﻿using System.Collections.Generic;

namespace TrickingLibrary.API.Forms
{
    public class TrickForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public IEnumerable<int> Prerequisites { get; set; }
        public IEnumerable<int> Progressions { get; set; }
        public IEnumerable<int> Categories { get; set; }
    }
}