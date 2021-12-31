using System;

using System.Security.Cryptography;
using System.Text;

namespace KaDiFi.Helpers
{
    public static class Helpers
    {
        public static string Mask(this string text)
        {
            var textConversion = Encoding.UTF8.GetBytes(text);
            var additionalKeys = new byte[] { 0x0, 0x1, 0x2, 0x3 };
            return Convert.ToBase64String(
                ProtectedData.Protect(textConversion, additionalKeys, DataProtectionScope.LocalMachine));
        }

        public static string UnMask(this string text)
        {
            var textConversion = Convert.FromBase64String(text);
            var additionalKeys = new byte[] { 0x0, 0x1, 0x2, 0x3 };
            return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(textConversion, additionalKeys, DataProtectionScope.CurrentUser));
        }



    }
}
