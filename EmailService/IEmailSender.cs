using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public interface IEmailSender
    {
        public void SendEmail(Message message);
        public Task SendEmailAsync(Message message);
    }
}
