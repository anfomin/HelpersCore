using System.Net.Mail;

namespace HelpersCore;

/// <summary>
/// Service for sending email messages.
/// </summary>
public interface IMailService
{
	/// <summary>
	/// Sends email message.
	/// </summary>
	/// <param name="message">Message to send.</param>
	/// <returns>Message identifier.</returns>
	Task<string> SendMailAsync(MailMessage message, CancellationToken cancellationToken = default);
}