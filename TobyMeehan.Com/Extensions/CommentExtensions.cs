using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Extensions
{
    public static class CommentExtensions
    {
        private static string Pluralise(int unitTime, string timeUnit)
        {
            return $"{unitTime} {timeUnit}{(unitTime == 1 ? "" : "s")}";
        }

        public static string DateString(this Comment comment)
        {
            DateTime date = comment.Sent;
            TimeSpan timeSpan = DateTime.Now - date;
            string interval;

            switch (timeSpan)
            {
                case TimeSpan t when t < TimeSpan.FromMinutes(1):

                    interval = Pluralise(timeSpan.Seconds, "second");

                    break;
                case TimeSpan t when t < TimeSpan.FromHours(1):

                    interval = Pluralise(timeSpan.Minutes, "minute");

                    break;
                case TimeSpan t when t < TimeSpan.FromDays(1):

                    interval = Pluralise(timeSpan.Hours, "hour");

                    break;
                case TimeSpan t when t < TimeSpan.FromDays(7):

                    interval = Pluralise(timeSpan.Days, "day");

                    break;
                case TimeSpan t when t < TimeSpan.FromDays(365):

                    interval = Pluralise(timeSpan.Days / 7, "week");

                    break;
                default:

                    interval = Pluralise(timeSpan.Days / 365, "year");

                    break;
            }

            return $"{interval} ago";
        }
    }
}
