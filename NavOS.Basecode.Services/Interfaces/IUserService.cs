using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.ServiceModels;
using static NavOS.Basecode.Resources.Constants.Enums;

namespace NavOS.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userid, string password, ref User user);
        void AddUser(UserViewModel model);
    }
}
