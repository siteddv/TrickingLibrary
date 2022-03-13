using TrickingLibrary.Models.Moderation;

namespace TrickingLibrary.API.Forms
{
    public class ReviewForm
    {
        public string Comment { get; set; }
        public ReviewStatus Status { get; set; }
    }
}