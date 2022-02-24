using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer.Localisation;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Extensions
{
    public static class CommentExtensions
    {
        public static string DateString(this Comment comment)
        {
            TimeSpan timeSpan = DateTime.Now - comment.Sent;

            return $"{timeSpan.Humanize(maxUnit: TimeUnit.Year)} ago";
        }
    }
}
