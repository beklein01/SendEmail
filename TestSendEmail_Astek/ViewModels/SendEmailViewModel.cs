using System;
using System.ComponentModel.DataAnnotations;

namespace TestSendEmail_Astek.ViewModels
{
    public class SendEmailViewModel
    {
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public String Email { get; set; }

        [Required]
        public String FullName { get; set; }
    }
}