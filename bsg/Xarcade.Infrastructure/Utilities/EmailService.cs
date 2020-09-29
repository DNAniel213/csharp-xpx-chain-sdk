using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;

namespace Xarcade.Infrastructure.Utilities
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Send(Email emailDetails)
        {
            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(this.configuration["SmtpEmailSettings:SmtpHost"],
                            int.Parse(this.configuration["SmtpEmailSettings:SmtpPort"]),
                            SecureSocketOptions.StartTls);
                smtp.Authenticate(this.configuration["SmtpEmailSettings:SmtpUser"], this.configuration["SmtpEmailSettings:SmtpPass"]);
                smtp.Send(GetEmailMessage(emailDetails));
                smtp.Disconnect(true);
            }
            catch (Exception e)
            {
                // TODO: Log here
                Console.WriteLine(e);
                return false;
            }
            

            return true;
        }

        private MimeMessage VerificationEmailMessage(Email emailDetails)
        {
            var message = "";
            if (!string.IsNullOrEmpty(emailDetails.Origin))
            {
                var verifyUrl = $"{emailDetails.Origin}/account/verify-email?token={emailDetails.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{emailDetails.VerificationToken}</code></p>";
            }

            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(this.configuration["SmtpEmailSettings:From"]),
                Subject = "Xarcade Account Registration - Verify Email",
                Body = new TextPart(TextFormat.Html) { Text = message },
            };
            email.To.Add(MailboxAddress.Parse(emailDetails.To));

            return email;
        }

        private MimeMessage PasswordResetEmailMessage(Email emailDetails)
        {
            var message = "";
            if (!string.IsNullOrEmpty(emailDetails.Origin))
            {
                var resetUrl = $"{emailDetails.Origin}/account/reset-password?token={emailDetails.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{emailDetails.ResetToken}</code></p>";
            }

            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(this.configuration["SmtpEmailSettings:From"]),
                Subject = "Xarcade Acccount - Reset Password",
                Body = new TextPart(TextFormat.Html) { Text = message },
            };
            email.To.Add(MailboxAddress.Parse(emailDetails.To));

            return email;
        }

        private MimeMessage AlreadyRegisteredEmailMessage(Email emailDetails)
        {
            var message = "";
            if (!string.IsNullOrEmpty(emailDetails.Origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{emailDetails.Origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(this.configuration["SmtpEmailSettings:From"]),
                Subject = "Xarcade Acccount - Already Registered",
                Body = new TextPart(TextFormat.Html) { Text = message },
            };
            email.To.Add(MailboxAddress.Parse(emailDetails.To));

            return email;
        }

        private MimeMessage GetEmailMessage(Email emailDetails)
        {
            switch (emailDetails.Type)
            {
                case EmailType.Verification:
                    return VerificationEmailMessage(emailDetails);
                case EmailType.PasswordReset:
                    return PasswordResetEmailMessage(emailDetails);
                case EmailType.AlreadyRegistered:
                    return AlreadyRegisteredEmailMessage(emailDetails);
                default:
                    return null;
            }
            
        }
    }
}
