using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace AppointmentScheduler.Utility
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        
        {
            MailjetClient client = new MailjetClient("3aff0a04a0bb86efdd08755fcc1fe2de", "0414f0319268efa45e50c11579054a52")
            {

            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
           .Property(Send.FromEmail, "valli2nayagam@ymail.com")
           .Property(Send.FromName, "Appointment Scheduler")
           .Property(Send.Subject, subject)
           .Property(Send.HtmlPart, htmlMessage)
           .Property(Send.Recipients, new JArray {
                new JObject {
                 {"Email", email}
                 }
               });
            MailjetResponse response = await client.PostAsync(request);


        }
    }
}
