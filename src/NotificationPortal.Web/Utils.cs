using System;

namespace NotificationPortal.Web
{
    public static class Utils
    {
        public static string FormatDateTime(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
