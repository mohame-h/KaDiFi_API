using System;
using System.Net;
using System.Net.Mail;

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
                message.From = new MailAddress("no-reply@KeDiFi.sys");
                //message.From = new MailAddress("mhdraz98@gmail.com");
                message.To.Add(new MailAddress(toMail));
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlString;

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("mhdraz98@gmail.com", "faQer5005");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }



    }
}
