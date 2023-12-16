namespace TrafficEscape.Services
{
    using FluentEmail.Core;
    using FluentEmail.MailKitSmtp;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Options;
    using TrafficEscape.Services.Interfaces;

    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }


        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await new Email().To(toEmail).Subject(subject).Body(message).SendAsync();
        }
    }
}
