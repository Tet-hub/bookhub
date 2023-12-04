using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NavOS.Basecode.Resources.Constants.Enums;

namespace NavOS.Basecode.Services.Interfaces
{
    public interface IAdminService
    {
        LoginResult AuthenticateAdmin(string email, string password, ref Admin admin);
        void AddAdmin(AdminViewModel model, string user);
        AdminViewModel GetAdmin(string adminId);
        bool CheckEmailExist(AdminViewModel adminViewModel);
        List<AdminViewModel> GetAllAdmins();
        bool DeleteAdmin(string adminId);
        bool EditAdmin(AdminViewModel adminViewModel, string user);
        bool InsertToken(AdminViewModel adminViewModel, string host);
        bool CheckQueryParamater(string AdminId, string Token);
        bool ChangePassword(AdminViewModel adminViewModel);
        Task<bool> CheckEmailValidAsync(string Email);
        List<AdminViewModel> GetAllAdminWithSearch(string searchQuery);

    }
}
