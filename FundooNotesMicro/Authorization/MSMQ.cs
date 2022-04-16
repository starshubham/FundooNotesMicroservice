using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using Experimental.System.Messaging;
using System.Linq;

namespace UserService.Authorization
{
    public class MSMQ
    {
        MessageQueue messageQueue = new MessageQueue();
        public void MSMQSender(string token)
        {
            messageQueue.Path = @".\private$\Token";//for windows path

            if (!MessageQueue.Exists(messageQueue.Path))
            {

                MessageQueue.Create(messageQueue.Path);

            }
            messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            //register the Method to the event 
            messageQueue.ReceiveCompleted += MessageQueue_ReceiveCompleted;
            messageQueue.Send(token);
            messageQueue.BeginReceive();
            messageQueue.Close();
        }

        private void MessageQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var message = messageQueue.EndReceive(e.AsyncResult);
            string token = message.Body.ToString();
            try
            {
                MailMessage mailmessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("demoshubham4567@gmail.com", "Shubham@2312"),
                    EnableSsl = true,
                };
                mailmessage.From = new MailAddress("demoshubham4567@gmail.com", "Shubham@2312");

                mailmessage.To.Add(new MailAddress("demoshubham4567@gmail.com"));
                mailmessage.Body = "Copy the token provided here to reset your password :  " + token;
                mailmessage.Subject = "Fundoo Notes Reset Password Link";
                smtpClient.Send(mailmessage);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }
}
