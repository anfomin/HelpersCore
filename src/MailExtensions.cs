using System.Net.Mail;

namespace HelpersCore;

public static class MailExtensions
{
	/// <summary>
	/// Sends email message.
	/// </summary>
	/// <param name="from">From address for message. If <c>null</c> then sends from default address.</param>
	/// <param name="to">Destination addresses.</param>
	/// <param name="subject">Message subject.</param>
	/// <param name="bodyHtml">Message body in HTML format.</param>
	/// <returns>Message identifier.</returns>
	public static Task<string> SendMailAsync(this IMailService mailService, MailAddress? from, IEnumerable<MailAddress> to, string subject, string bodyHtml, CancellationToken cancellationToken = default)
	{
		MailMessage message = new()
		{
			Body = bodyHtml,
			IsBodyHtml = true
		};
		if (from != null)
			message.From = from;
		foreach (var address in to)
			message.To.Add(address);
		return mailService.SendMailAsync(message, cancellationToken);
	}

	/// <summary>
	/// Sends email message.
	/// </summary>
	/// <param name="from">From address for message. If <c>null</c> then sends from default address.</param>
	/// <param name="to">Destination address.</param>
	/// <param name="subject">Message subject.</param>
	/// <param name="bodyHtml">Message body in HTML format.</param>
	/// <returns>Message identifier.</returns>
	public static Task<string> SendMailAsync(this IMailService mailService, MailAddress? from, MailAddress to, string subject, string bodyHtml, CancellationToken cancellationToken = default)
		=> mailService.SendMailAsync(from, [to], subject, bodyHtml, cancellationToken);

	/// <summary>
	/// Sends email message from default address.
	/// </summary>
	/// <param name="to">Destination addresses.</param>
	/// <param name="subject">Message subject.</param>
	/// <param name="bodyHtml">Message body in HTML format.</param>
	/// <returns>Message identifier.</returns>
	public static Task<string> SendMailAsync(this IMailService mailService, IEnumerable<MailAddress> to, string subject, string bodyHtml,
		CancellationToken cancellationToken = default)
		=> mailService.SendMailAsync(null, to, subject, bodyHtml, cancellationToken);

	/// <summary>
	/// Sends email message from default address.
	/// </summary>
	/// <param name="to">Destination address.</param>
	/// <param name="subject">Message subject.</param>
	/// <param name="bodyHtml">Message body in HTML format.</param>
	/// <returns>Message identifier.</returns>
	public static Task<string> SendMailAsync(this IMailService mailService, MailAddress to, string subject, string bodyHtml, CancellationToken cancellationToken = default)
		=> mailService.SendMailAsync(null, [to], subject, bodyHtml, cancellationToken);
}