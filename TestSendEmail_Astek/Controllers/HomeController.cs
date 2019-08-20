using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestSendEmail_Astek.EmailTemplates;
using TestSendEmail_Astek.Models;
using TestSendEmail_Astek.Controllers.ViewModels;

namespace TestSendEmail_Astek.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SendEmail(SendEmailViewModel vm)
        {
            //Send the email

           // ILogger logger = new Logger
            EmailHelper< BienvenueEmailModel> sender = new EmailHelper<BienvenueEmailModel>(null);

            string template = "BienvenueEmail.cshtml";
            string fromEmail = "klein.houzin_csm@outlook.com";
            string fromName = "ARCANIA";

            BienvenueEmailModel model = new BienvenueEmailModel(fromEmail, fromName, vm.Email, vm.FullName, "Bienvenue", "IT");
            
            sender.SendEmail(model, template);

            return View(vm);
        }
      
    }
}