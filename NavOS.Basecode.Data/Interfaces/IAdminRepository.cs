using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Interfaces
{
    public interface IAdminRepository
    {
        public IQueryable<Admin> GetAdmins();
        public void AddAdmin(Admin admin);
        public bool AdminExists(string email);
        public bool AdminExists_v2(string adminId, string token);
        public void DeleteAdmin(Admin admin);
        public void UpdateAdmin(Admin admin);
    }
}
