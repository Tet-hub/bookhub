using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Interfaces
{
    public interface IEmailSender
    {
        public void PasswordReset(string email, string host, string adminName, string adminId, string token);
    }
}
