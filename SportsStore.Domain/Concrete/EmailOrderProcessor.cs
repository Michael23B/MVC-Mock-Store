using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "example@email.com";
        public string MailFromAddress = "example@email.com";
        public bool UseSsl = true;
        public string Username = "Your_Username";
        public string Password = "Your_Password";
        public string ServerName = "Your_Server";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = AppDomain.CurrentDomain.BaseDirectory + "App_Data";

        public void ReadFromFile(string path)   //Settings are loaded from a plain text file with 9 lines, one for each field.
                                                //Called in EmailOrderProcessor constructor or call it from you own IOrderProcessor
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            if (lines == null || lines.Count() < 9) return;

            MailToAddress = lines[0];
            MailFromAddress = lines[1];
            UseSsl = bool.Parse(lines[2]);
            Username = lines[3];
            Password = lines[4];
            ServerName = lines[5];
            ServerPort = int.Parse(lines[6]);
            WriteAsFile = bool.Parse(lines[7]);
            FileLocation = AppDomain.CurrentDomain.BaseDirectory + lines[8];
        }
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
            settings.ReadFromFile(AppDomain.CurrentDomain.BaseDirectory + "App_Data/secret_settings.txt");
            //Uncomment above to set EmailSettings from a file (and keep passwords out of code)
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder().AppendLine("A new order has been submitted.").AppendLine("---").AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c})\n", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}\n", cart.ComputeTotalValue())
                .AppendLine("---")
                .AppendLine("Ship to:")
                .AppendLine(shippingInfo.Name)
                .AppendLine(shippingInfo.Line1)
                .AppendLine(shippingInfo.Line2 ?? "")
                .AppendLine(shippingInfo.Line3 ?? "")
                .AppendLine(shippingInfo.City)
                .AppendLine(shippingInfo.State)
                .AppendLine(shippingInfo.Country)
                .AppendLine(shippingInfo.Zip ?? "")
                .AppendLine("---")
                .AppendFormat("Gift wrap: {0}", shippingInfo.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, emailSettings.MailToAddress, "New order submitted!", body.ToString());

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
    }
}
