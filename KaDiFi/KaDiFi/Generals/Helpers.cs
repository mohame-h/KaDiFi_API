using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace KaDiFi.Helpers
{
    public static class StaticHelpers
    {
        //public static string Mask(this string text)
        //{
        //    var textConversion = Encoding.UTF8.GetBytes(text);
        //    var additionalKeys = new byte[] { 0x0, 0x1, 0x2, 0x3 };
        //    return Convert.ToBase64String(
        //        ProtectedData.Protect(textConversion, additionalKeys, DataProtectionScope.LocalMachine));
        //}

        //public static string UnMask(this string text)
        //{
        //    var textConversion = Convert.FromBase64String(text);
        //    var additionalKeys = new byte[] { 0x0, 0x1, 0x2, 0x3 };
        //    return Encoding.Unicode.GetString(
        //        ProtectedData.Unprotect(textConversion, additionalKeys, DataProtectionScope.LocalMachine));
        //}

        public static bool validateSpecialChars(string text)
        {
            var passwordChars = text.ToCharArray();
            int specialChars = 0;
            foreach (var c in passwordChars)
            {
                if (!char.IsLetterOrDigit(c))
                    specialChars++;
            }

            return specialChars > 0;
        }

        public static bool sendEmail(string toMail, string subject, string htmlString)
        {
            try
            {

                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("no-reply@KeDiFi.sys");
                message.To.Add(new MailAddress(toMail));
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlString;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("mhdraz98@gmail.com", "Aaaaa#1234");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }



    }
}
