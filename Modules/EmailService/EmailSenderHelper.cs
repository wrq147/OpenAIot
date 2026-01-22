using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using TemplateAction.Core;
using AuthService;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Common.Json;

namespace EmailService
{
    /// <summary>
    /// 邮件发送帮助类
    /// </summary>
    public class EmailSenderHelper
    {
        private ITAServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public EmailSenderHelper(ITAServiceProvider serviceProvider, ILogger<EmailSenderHelper> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task<EmailConfig> GetEmailConfig()
        {
            var tmpstr = await _serviceProvider.GetService<ConfigBLL>().SelectConfigByKey("system.email");
            return System.Text.Json.JsonSerializer.Deserialize<EmailConfig>(tmpstr, MyDefaultTextJsonConfig.DefaultOptions);
        }
        public async Task<bool> SendPlainEmail(string to, string subject, string message, params string[] attachments)
        {
            List<string> tolist = new List<string>();
            tolist.Add(to);
            return await SendPlainEmail(tolist, subject, message, attachments);
        }
        /// <summary>
        /// 发送纯文本
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public async Task<bool> SendPlainEmail(List<string> to, string subject, string message, params string[] attachments)
        {
            try
            {
                var tmpconfig = await GetEmailConfig();
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(tmpconfig.email_from_name, tmpconfig.email_from));
                emailMessage.To.AddRange(to.Select(e => new MailboxAddress(e, e)));
                emailMessage.Subject = subject;

                var alternative = new Multipart("alternative");
                alternative.Add(new TextPart("plain") { Text = message });

                if (attachments != null)
                {
                    foreach (string f in attachments)
                    {
                        var attachment = new MimePart()//("image", "png")
                        {
                            Content = new MimeContent(File.OpenRead(f), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = Path.GetFileName(f)
                        };
                        alternative.Add(attachment);
                    }
                }
                emailMessage.Body = alternative;

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(tmpconfig.email_host, tmpconfig.email_post, (SecureSocketOptions)tmpconfig.ssl);
                    await client.AuthenticateAsync(tmpconfig.email_from, tmpconfig.email_password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError("发送邮件异常:" + ex.Message);
                return false;
            }

        }
        public async Task<bool> SendHtmlEmail(string to, string subject, string message, params string[] attachments)
        {
            List<string> tolist = new List<string>();
            tolist.Add(to);
            return await SendHtmlEmail(tolist, subject, message, attachments);
        }
        /// <summary>
        /// 发送html
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public async Task<bool> SendHtmlEmail(List<string> to, string subject, string message, params string[] attachments)
        {
            try
            {
                var tmpconfig = await GetEmailConfig();
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(tmpconfig.email_from_name, tmpconfig.email_from));
                emailMessage.To.AddRange(to.Select(e => new MailboxAddress(e, e)));
                emailMessage.Subject = subject;

                var alternative = new Multipart("alternative");
                alternative.Add(new TextPart("html") { Text = message });

                if (attachments != null)
                {
                    foreach (string f in attachments)
                    {
                        var attachment = new MimePart()//("image", "png")
                        {
                            Content = new MimeContent(File.OpenRead(f), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = Path.GetFileName(f)
                        };
                        alternative.Add(attachment);
                    }
                }
                emailMessage.Body = alternative;

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(tmpconfig.email_host, tmpconfig.email_post, (SecureSocketOptions)tmpconfig.ssl);
                    await client.AuthenticateAsync(tmpconfig.email_from, tmpconfig.email_password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError("发送邮件异常:" + ex.Message);
                return false;
            }
        }
    }
}
