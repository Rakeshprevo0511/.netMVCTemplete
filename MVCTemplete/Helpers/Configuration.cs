using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCTemplete.Helpers
{
    public class Configuration
    {
        public static DateTime ConvertToIST(DateTime utcDateTime)
        {
            TimeZoneInfo istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istZone);
        }
    }
}