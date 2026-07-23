using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MVCTemplete.Helpers
{

    public static class MailHelper
    {
        #region SMTP Configuration

        private static readonly string Host = "smtp.gmail.com";
        private static readonly int Port = 587;

        private static readonly string Username = "yourmail@gmail.com";
        private static readonly string Password = "AppPassword";

        private static readonly bool EnableSSL = true;

        #endregion

        #region Create SMTP Client

        public static SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = Host,
                Port = Port,
                EnableSsl = EnableSSL,
                Credentials = new NetworkCredential(Username, Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        #endregion

        #region Create Mail Message

        public static MailMessage CreateMailMessage(
            string to,
            string subject,
            string body,
            bool isHtml = true)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(Username);

            mail.To.Add(to);

            mail.Subject = subject;

            mail.Body = body;

            mail.IsBodyHtml = isHtml;

            return mail;
        }

        #endregion

        #region Basic Email

        public static bool SendEmail(
            string to,
            string subject,
            string body)
        {
            try
            {
                using (MailMessage mail = CreateMailMessage(to, subject, body))
                using (SmtpClient smtp = CreateSmtpClient())
                {
                    smtp.Send(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region HTML Email

        public static bool SendHtmlEmail(
            string to,
            string subject,
            string htmlBody)
        {
            try
            {
                using (MailMessage mail = CreateMailMessage(to, subject, htmlBody, true))
                using (SmtpClient smtp = CreateSmtpClient())
                {
                    smtp.Send(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Async Email

        public static async System.Threading.Tasks.Task<bool> SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            try
            {
                using (MailMessage mail = CreateMailMessage(to, subject, body))
                using (SmtpClient smtp = CreateSmtpClient())
                {
                    await smtp.SendMailAsync(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Multiple Recipients

        public static bool SendEmail(
            List<string> recipients,
            string subject,
            string body)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(Username);

                foreach (string email in recipients)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                        mail.To.Add(email);
                }

                mail.Subject = subject;

                mail.Body = body;

                mail.IsBodyHtml = true;

                using (SmtpClient smtp = CreateSmtpClient())
                {
                    smtp.Send(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region CC BCC

        public static bool SendEmail(
            string to,
            List<string> cc,
            List<string> bcc,
            string subject,
            string body)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(Username);

                mail.To.Add(to);

                if (cc != null)
                {
                    foreach (string item in cc)
                        mail.CC.Add(item);
                }

                if (bcc != null)
                {
                    foreach (string item in bcc)
                        mail.Bcc.Add(item);
                }

                mail.Subject = subject;

                mail.Body = body;

                mail.IsBodyHtml = true;

                using (SmtpClient smtp = CreateSmtpClient())
                {
                    smtp.Send(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Attachment

        public static bool SendEmailWithAttachment(
            string to,
            string subject,
            string body,
            string attachmentPath)
        {
            try
            {
                MailMessage mail = CreateMailMessage(to, subject, body);

                if (File.Exists(attachmentPath))
                {
                    mail.Attachments.Add(new Attachment(attachmentPath));
                }

                using (SmtpClient smtp = CreateSmtpClient())
                {
                    smtp.Send(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Multiple Attachments

        public static bool SendEmailWithAttachments(
            string to,
            string subject,
            string body,
            List<string> attachments)
        {
            try
            {
                MailMessage mail = CreateMailMessage(to, subject, body);

                foreach (string file in attachments)
                {
                    if (File.Exists(file))
                        mail.Attachments.Add(new Attachment(file));
                }

                using (SmtpClient smtp = CreateSmtpClient())
                {
                    smtp.Send(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Validation

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        #endregion

        public static bool SendOTPEmail(string to, string otp)
        {
            string subject = "OTP Verification";

            string body = $@"
            <html>
            <body style='font-family:Arial'>
                <h2>OTP Verification</h2>

                <p>Your One-Time Password is:</p>

                <h1 style='color:#0d6efd'>{otp}</h1>

                <p>This OTP is valid for 10 minutes.</p>

                <p>Please do not share this OTP with anyone.</p>

                <br/>

                <b>Regards,</b><br/>
                Your Company
            </body>
            </html>";

            return SendHtmlEmail(to, subject, body);
        }
        public static bool SendWelcomeEmail(string to, string userName)
        {
            string subject = "Welcome to Our Application";

            string body = $@"
            <html>
            <body style='font-family:Arial'>

                <h2>Welcome {userName}!</h2>

                <p>Your account has been created successfully.</p>

                <p>We're excited to have you with us.</p>

                <br/>

                <b>Regards,</b><br/>
                Support Team

            </body>
            </html>";

            return SendHtmlEmail(to, subject, body);
        }
        public static bool SendPasswordResetEmail(string to, string resetLink)
        {
            string subject = "Reset Your Password";

            string body = $@"
            <html>
            <body style='font-family:Arial'>

                <h2>Password Reset</h2>

                <p>Click the button below to reset your password.</p>

                <br/>

                <a href='{resetLink}'
                   style='
                   background:#0d6efd;
                   color:white;
                   padding:12px 20px;
                   text-decoration:none;
                   border-radius:5px'>
                   Reset Password
                </a>

                <br/><br/>

                <p>If you didn't request this, please ignore this email.</p>

                <br/>

                Regards,<br/>
                Support Team

            </body>
            </html>";

            return SendHtmlEmail(to, subject, body);
        }
        public static bool SendVerificationEmail( string to, string verificationLink)
        {
            string subject = "Verify Your Email";

            string body = $@"
    <html>
    <body style='font-family:Arial'>

        <h2>Email Verification</h2>

        <p>Please verify your email address by clicking the button below.</p>

        <br/>

        <a href='{verificationLink}'
           style='
           background:green;
           color:white;
           padding:12px 20px;
           text-decoration:none;
           border-radius:5px'>
           Verify Email
        </a>

        <br/><br/>

        <p>Thank you for registering.</p>

        <br/>

        Regards,<br/>
        Support Team

    </body>
    </html>";

            return SendHtmlEmail(to, subject, body);
        }
    }
}