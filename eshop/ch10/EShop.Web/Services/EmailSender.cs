using Microsoft.AspNetCore.Identity.UI.Services;

namespace EShop.Web.Services;

public class EmailSender(ILogger<EmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation("寄信給 {Email}：{Subject}", email, subject);
        return Task.CompletedTask;           // 開發階段先不真的寄信
    }
}
