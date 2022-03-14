using System;
using System.Linq;
using System.Linq.Expressions;
using TrickingLibrary.Models.Moderation;

namespace TrickingLibrary.API.ViewModels
{
    public static class ModerationItemViewModel
    {
        public static readonly Func<ModerationItem, object> Create = Projection.Compile();

        public static Expression<Func<ModerationItem, object>> Projection =>
            modItem => new
            {
                modItem.Id,
                modItem.Current,
                modItem.Target,
                modItem.Type,
                Comments = modItem.Comments.AsQueryable().Select(CommentViewModel.Projection).ToList(),
                Reviews = modItem.Reviews.AsQueryable().Select(ReviewViewModel.Projection).ToList(),
            };
    }
}