using System;
using System.IO;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ScavengeRUs.Services;
using System.Net;
using ScavengeRUs.Models.Enums;

/// <summary>
/// a class full of miscellaneous helper functions that we have yet to find a better place for
/// </summary>
public class Functions
{
	/// <summary>
	/// most major cell service providers provide a way to send SMS via email.
	/// just send an email to [phone number]@[carrier gateway url]
	/// and the owner of that phone number will recieve your email as a text message
	/// </summary>
	private static readonly Dictionary<Carrier, string> _carrierToGateway = new Dictionary<Carrier, string>{
		{ Carrier.ATT, "txt.att.net" },
		{ Carrier.BoostMobile, "sms.myboostmobile.com" }, // untested
		{ Carrier.Charter, "vtext.com" },
		{ Carrier.Cricket, "sms.cricketwireless.net" }, // untested
		{ Carrier.Sprint, "messaging.sprintpcs.net" }, // untested
		{ Carrier.StraightTalk, "vtext.com" }, // uses verizon's url, contrary to what documentation says
		{ Carrier.TMobile, "tmomail.net" },
		{ Carrier.TracFone, "txt.att.net" },
		{ Carrier.Verizon, "vtext.com" },
		{ Carrier.VirginMobile, "vmobl.com" } // untested
	};

	/// <summary>
	/// a set of key-value pairs to track settings to control the behaviour of this class
	/// </summary>
	private static IConfiguration? _configuration;

    /// <summary>
    /// sets the configuration for the Functions class
    /// </summary>
    /// <param name="configuration"></param>
    public static void SetConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
	}


    // these fields are configurable values for our smtp client and secrets
	private static readonly string _applicationEmailAddress = "etsubuchunt98@gmail.com";
	private static readonly string smtpURL = "smtp.gmail.com";
	private static readonly int smtpPort = 587;
	private static readonly string secretsFileName = "gmail.txt";


	/// <summary>
	/// uses an SMTP client running an existing buc hunt email and google's SMTP service to send emails to recipients email address
	/// </summary>
	/// <param name="recipientEmail"></param>
	/// <param name="subject"></param>
	/// <param name="body"></param>
	/// <returns></returns>
	public static async Task SendEmail(string recipientEmail, string subject, string body)
	{
		// our SMTP server is tied to an gmail account and sends emails on behalf of this account.
		// we keep the password in a file by itself not tracked by source control. if you are part
		// of a future group of SE1 students and need the login info, please contact
		// either SCUTTW@ETSU.EDU, or grantscutt2@gmail.com, preferably from a school account
		string secretsFilePath = Path.Combine(Directory.GetCurrentDirectory(), secretsFileName);
		string emailPassword = File.ReadAllText(secretsFilePath).Trim();

		// Configure SMTP client
		using (SmtpClient client = new SmtpClient(smtpURL, smtpPort)) {
			client.UseDefaultCredentials = false;
			client.Credentials = new NetworkCredential(_applicationEmailAddress, emailPassword);
			client.EnableSsl = true;

			// Create email message
			using (MailMessage message = new MailMessage(_applicationEmailAddress, recipientEmail)) {
				message.Subject = subject;
				message.Body = body;
				message.IsBodyHtml = true;

				// NOTE: can throw SmtpException, but we don't really know what to do if it does, so we don't catch it
				client.Send(message);
			}
		}
	}


	/// <summary>
	/// sends an email to a phone number to be recieved as SMS
	/// </summary>
	/// <param name="carrier"></param>
	/// <param name="recipientPhNumber"></param>
	/// <param name="body"></param>
	/// <returns></returns>
	public static async Task SendSMS(Carrier carrier, string recipientPhNumber, string body)
	{
		string recipientAddress = $"{recipientPhNumber}@{_carrierToGateway[carrier]}";
		await SendEmail(recipientAddress, "", body);
	}
}
