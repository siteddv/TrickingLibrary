using System;
using System.Linq.Expressions;
using TrickingLibrary.Models;

namespace TrickingLibrary.API.ViewModels
{
    public static class CommentViewModel
    {
        public static readonly Func<Comment, object> Create = Projection.Compile();

        public static Expression<Func<Comment, object>> Projection =>
            comment => new
            {
                comment.Id,
                comment.ParentId,
                comment.Content,
                comment.HtmlContent,
            };
    }
}