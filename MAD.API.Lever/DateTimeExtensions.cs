using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.API.Lever
{
    public static class DateTimeExtensions
    {
        public static DateTime Epoch { get; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double GetMillisecondsSinceEpoch(this DateTime dateTime)
        {
            return (dateTime - Epoch).TotalMilliseconds;
        }
    }
}
