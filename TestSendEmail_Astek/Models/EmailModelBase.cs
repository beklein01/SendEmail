using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace TestSendEmail_Astek.Models
{
    /// <summary>
    /// Tous les Models d'email doivent dériver de cette classe
    /// </summary>
    public class EmailModelBase
    {
        public string ToEmail { get; set; }

        public string FullName { get; set; }

        public string Subject { get; set; }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string Preheader { get; set; }

        public List<Attachment> Attachments { get; set; }


        /// <summary>
        /// Créer une base d'email
        /// </summary>
        /// <param name="fromEmail">Email de l'expéditeur</param>
        /// <param name="fromName">Nom de l'expéditeur</param>
        /// <param name="toEmail">Email du destinataire</param>
        /// <param name="fullName">Nom du destinataire</param>
        /// <param name="subject">Sujet de l'Email</param>
        /// <param name="preheader">En tête de l'email</param>
        /// <param name="preheader">En tête de l'email</param>
        public EmailModelBase(string fromEmail, string fromName, string toEmail, string fullName, string subject,
            string preheader = "", List<Attachment> attachments = null)
        {
            this.FromEmail = fromEmail;

            this.FromName = fromName;

            this.ToEmail = toEmail;

            this.FullName = fullName;

            this.Subject = subject;

            this.Preheader = preheader;
            
            if (attachments == null)
            {
                this.Attachments = new List<Attachment>();
            }
            else
            {
                this.Attachments = attachments;
            }

        }

    }
}