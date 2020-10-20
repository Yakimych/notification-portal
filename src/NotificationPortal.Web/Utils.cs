using System;
using System.Text;

namespace NotificationPortal.Web
{
    public static class Utils
    {
        public static string FormatDateTime(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // TODO: Unit tests/property tests with regex: "[a-zA-Z0-9-_.~%]+"
        // https://firebase.google.com/docs/cloud-messaging/send-message#send_to_a_topic
        public static string Base64UrlEncode(this string plainText) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        public static string Base64UrlDecode(this string base64EncodedData)
        {
            string incoming = base64EncodedData.Replace('_', '/').Replace('-', '+');
            switch(base64EncodedData.Length % 4) {
                case 2:
                    incoming += "==";
                     break;
                case 3:
                    incoming += "=";
                    break;
            }

            byte[] bytes = Convert.FromBase64String(incoming);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
