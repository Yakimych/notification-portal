using System;
using System.Text;
// using Microsoft.AspNetCore.WebUtilities;

namespace NotificationPortal.Web
{
    public static class Utils
    {
        public static string FormatDateTime(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // TODO: Unit tests/property tests with regex: "[a-zA-Z0-9-_.~%]+"
        // https://firebase.google.com/docs/cloud-messaging/send-message#send_to_a_topic
        // public static string Base64UrlEncode(this string plainText) {
        //     var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        //     return WebEncoders.Base64UrlEncode(plainTextBytes);
        // }
        //
        // public static string Base64UrlDecode(this string base64EncodedData) {
        //     var base64EncodedBytes = WebEncoders.Base64UrlDecode(base64EncodedData);
        //     return Encoding.UTF8.GetString(base64EncodedBytes);
        // }

        public static string Base64UrlEncode(this string plainText) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        // TODO: Refactor before committing
        public static string Base64UrlDecode(this string base64EncodedData) {
            string incoming = base64EncodedData.Replace('_', '/').Replace('-', '+');
            switch(base64EncodedData.Length % 4) {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(incoming);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
