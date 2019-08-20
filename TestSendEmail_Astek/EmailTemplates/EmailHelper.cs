using Microsoft.Extensions.Logging;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using TestSendEmail_Astek.Models;

namespace TestSendEmail_Astek.EmailTemplates
{
    /// <summary>
    /// Permet d'envoyer un email d'une façon asynchrone. 
    /// Appeler SendEmail
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmailHelper<T> where T : EmailModelBase
    {
        private readonly string TemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["EmailTemplates"].ToString());

        private readonly string _sender = ConfigurationManager.AppSettings["SmtpServerEmail"].ToString(); 

        private readonly string _password = ConfigurationManager.AppSettings["SmtpServerPassword"].ToString();

        private readonly string _smtp = ConfigurationManager.AppSettings["SmtpServerHostname"].ToString();

        private readonly int _portSmtp = Int32.Parse(ConfigurationManager.AppSettings["SmtpServerPortNumber"].ToString());

        private readonly int _timeout = Int32.Parse(ConfigurationManager.AppSettings["SmtpServerTimeout"].ToString());


        private readonly ILogger _logger;

        /// <summary>
        /// Email helper pour envoyer des emails.
        /// </summary>
        /// <param name="logger">Passer le logger pour pouvoir implementer le log des erreurs et des informations</param>
        public EmailHelper(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Envoyer Email (d'une manière asynchrone) avec un template dans le dossier du template (appsettings EmailTemplatesFolder dans le web.config)
        /// </summary>
        /// <param name="model">Le model de l'email</param>
        /// <param name="template">Le nom complet du template. Ne pas oublier de déployer le dossier du template avec l'application</param>
        public void SendEmail(EmailModelBase model, string template)
        {
            AsyncMethodCaller caller = new AsyncMethodCaller(SendMailInSeperateThread);
            AsyncCallback callbackHandler = new AsyncCallback(AsyncCallback);
            caller.BeginInvoke(model, template, callbackHandler, null);
        }


        private delegate bool AsyncMethodCaller(EmailModelBase model, string template);

        private bool SendMailInSeperateThread(EmailModelBase model, string template)
        {
            SmtpClient smtpClient = null;
            MailMessage email = null;

            try
            {
                // Generate an email using a dynamic model
                String myEmailkey = Guid.NewGuid().ToString();

                var emailHtmlBody = RunCompile(TemplateFolderPath, template, myEmailkey, model);


                // Send the email
                email = new MailMessage()
                {
                    Body = emailHtmlBody,
                    IsBodyHtml = true,
                    Subject = model.Subject
                };

                email.From = new MailAddress(model.FromEmail, model.FromName);

                email.To.Add(new MailAddress(model.ToEmail, model.FullName));

                foreach (var item in model.Attachments)
                {
                    email.Attachments.Add(item);
                }

                smtpClient = new SmtpClient(_smtp);

                smtpClient.Port = _portSmtp;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;

                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential(_sender, _password);

                smtpClient.EnableSsl = true;

                smtpClient.Timeout = _timeout;

                smtpClient.Credentials = credentials;

                smtpClient.Send(email);


                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return false;
            }
            finally
            {
                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
                if (email != null)
                {
                    email.Dispose();
                }
            }

        }


        /// <summary>
        /// Generer un HTML document a partir d'un  template Razor et un model.
        /// </summary>
        /// <param name="rootpath">The path to the folder containing the Razor templates</param>
        /// <param name="templatename">The name of the Razor template (.cshtml)</param>
        /// <param name="templatekey">The template key used for caching Razor templates which is essential for improved performance</param>
        /// <param name="model">The model containing the information to be supplied to the Razor template</param>
        /// <returns></returns>
        public string RunCompile(string rootpath, string templatename, string templatekey, object model)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(rootpath) || string.IsNullOrEmpty(templatename) || model == null) return result;

            string templateFilePath = Path.Combine(rootpath, templatename);

            if (File.Exists(templateFilePath))
            {
                string template = File.ReadAllText(templateFilePath);

                if (string.IsNullOrEmpty(templatekey))
                {
                    templatekey = Guid.NewGuid().ToString();
                }
                result = Engine.Razor.RunCompile(template, templatekey, null, model);
            }
            return result;
        }

        private void AsyncCallback(IAsyncResult ar)
        {
            try
            {
                AsyncResult result = (AsyncResult)ar;
                AsyncMethodCaller caller = (AsyncMethodCaller)result.AsyncDelegate;
                caller.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "callback from email failed");
            }
        }
    }

}