using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BlogAspNet.Services
{
    public class SendEmail
    {
        public static void Send(string to, string subject, string body)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(to);
            mm.Subject = subject;
            mm.Body = body;
            mm.From = new MailAddress("noreply1.2.3replyno@gmail.com");
            mm.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;

            smtp.Credentials = new System.Net.NetworkCredential("noreply1.2.3replyno@gmail.com",
                "jo!@#123Abc");
            smtp.Send(mm);
        }
    }
}
