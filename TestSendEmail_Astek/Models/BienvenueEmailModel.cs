using System;

namespace TestSendEmail_Astek.Models
{
    public class BienvenueEmailModel: EmailModelBase
    {
        public string Department { get; set; }

        public BienvenueEmailModel(string fromEmail, string fromName, string toEmail, string fullName, String subject, string department):
            base(fromEmail, fromName, toEmail, fullName, subject)
        {
            this.Department = department;
        }

    }
}