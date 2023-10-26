using AutoMapper;
using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.Manager;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NavOS.Basecode.Resources.Constants.Enums;

namespace NavOS.Basecode.Services.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public AdminService(IAdminRepository repository, IMapper mapper)
        {
            _adminRepository = repository;
            _mapper = mapper;
        }
        public LoginResult AuthenticateAdmin(string email, string password, ref Admin admin)
        {
            admin = new Admin();
            var passwordKey = PasswordManager.EncryptPassword(password);
            admin = _adminRepository.GetAdmins().Where(x => x.AdminEmail == email &&
                                                            x.Password == passwordKey).FirstOrDefault();
            return admin != null ? LoginResult.Success : LoginResult.Failed;
        }

        public void AddAdmin(AdminViewModel model) 
        {
            var admin = new Admin();
            if(!_adminRepository.AdminExists(model.AdminEmail))
            {
                _mapper.Map(model, admin);
                admin.AdminId = Guid.NewGuid().ToString();
                admin.AdminName = model.AdminName;
                admin.AdminEmail = model.AdminEmail;
                admin.Password = PasswordManager.EncryptPassword(model.Password);
                admin.Role = "Master Admin";
                admin.Dob = model.Dob;
                admin.ContactNo = model.ContactNo;
                
                _adminRepository.AddAdmin(admin);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.AdminExists);
            }
        }
    }
}
