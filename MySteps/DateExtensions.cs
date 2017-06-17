using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace MySteps
{
    static class DateExtensions
    {
        public static NSDate ToNSDate(this DateTime date)
        {
            var baseDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var span = date.ToUniversalTime().Subtract(baseDate).TotalSeconds;
            return NSDate.FromTimeIntervalSinceReferenceDate(span);
        }

        public static DateTime ToDateTime(this NSDate date)
        {
            var baseDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return baseDate.AddSeconds(date.SecondsSinceReferenceDate);
        }

        public static DateTime ToDateTime(this NSDate date, NSCalendarUnit unitFlags)
        {
            var date2 = NSCalendar.CurrentCalendar.DateFromComponents(NSCalendar.CurrentCalendar.Components(unitFlags, date));
            var baseDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return baseDate.AddSeconds(date2.SecondsSinceReferenceDate);
        }
    }
}