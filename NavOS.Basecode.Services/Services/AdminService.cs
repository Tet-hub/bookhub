using AutoMapper;
using NavOS.Basecode.Data;
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
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="emailSender">The email sender.</param>
        public AdminService(IAdminRepository repository, IMapper mapper, IEmailSender emailSender)
        {
            _adminRepository = repository;
            _mapper = mapper;
            _emailSender = emailSender;
        }
        /// <summary>
        /// Authenticates the admin.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="admin">The admin.</param>
        /// <returns></returns>
        public LoginResult AuthenticateAdmin(string email, string password, ref Admin admin)
        {
            admin = new Admin();
            var passwordKey = PasswordManager.EncryptPassword(password);
            admin = _adminRepository.GetAdmins().Where(x => x.AdminEmail == email &&
                                                            x.Password == passwordKey).FirstOrDefault();
            return admin != null ? LoginResult.Success : LoginResult.Failed;
        }
        /// <summary>
        /// Adds the admin.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="user">The user.</param>
        /// <exception cref="System.IO.InvalidDataException"></exception>
        public void AddAdmin(AdminViewModel model, string user) 
        {
            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;
            var admin = new Admin();
            if(!_adminRepository.AdminExists(model.AdminEmail))
            {
                _mapper.Map(model, admin);
                admin.AdminId = Guid.NewGuid().ToString();
                admin.AdminName = model.AdminName;
                admin.AdminEmail = model.AdminEmail;
                admin.Password = PasswordManager.EncryptPassword(model.Password);
                admin.Role = "Admin";
                admin.Dob = model.Dob;
                admin.ContactNo = model.ContactNo;
                admin.AddedBy = user;
                admin.AddedTime = DateTime.Now;
                admin.UpdatedBy = user;
                admin.UpdatedTime = DateTime.Now;

                if (model.AdminProfile != null)
                {
					var coverImageFileName = Path.Combine(coverImagesPath, admin.AdminId) + ".png";
					using (var fileStream = new FileStream(coverImageFileName, FileMode.Create))
					{
						model.AdminProfile.CopyTo(fileStream);
					}

				}
				_adminRepository.AddAdmin(admin);
			}
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.AdminExists);
            }
        }

        /// <summary>
        /// Gets all admins.
        /// </summary>
        /// <returns></returns>
        public List<AdminViewModel> GetAllAdmins()
        {
            var url = "https://127.0.0.1:8080";
            var data = _adminRepository.GetAdmins().Select(s => new AdminViewModel
            {
                AdminId = s.AdminId,
                AdminEmail = s.AdminEmail,
                AdminName = s.AdminName,
                Role = s.Role,
                ContactNo = s.ContactNo,
                Dob = s.Dob,
                ImageUrl = (CheckAdminProfile(s.AdminId)) ? Path.Combine(url, s.AdminId + ".png") : Resources.Views.FileDir.AdminPicDefault,

            }).OrderByDescending(admin => admin.Role == "Master Admin")
              .ThenBy(admin => admin.Role)
              .ThenBy(admin => admin.AdminName)
              .ToList();
            return data;
        }

        /// <summary>
        /// Gets the admin.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        public AdminViewModel GetAdmin(string adminId)
        {
			var url = "https://127.0.0.1:8080";
			var admin = _adminRepository.GetAdmins().Where(x => x.AdminId == adminId).FirstOrDefault();

            if (admin != null)
            {
				AdminViewModel adminViewModel = new AdminViewModel();
				adminViewModel.AdminId = adminId;
				adminViewModel.AdminName = admin.AdminName;
				adminViewModel.AdminEmail = admin.AdminEmail;
				adminViewModel.ContactNo = admin.ContactNo;
				adminViewModel.Dob = admin.Dob;
				adminViewModel.Role = admin.Role;
				adminViewModel.ImageUrl = (CheckAdminProfile(adminId)) ? Path.Combine(url, admin.AdminId + ".png") : "#";

                return adminViewModel;
			}
			return null;

		}
        /// <summary>
        /// Deletes the admin.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        public bool DeleteAdmin(string adminId)
        {
            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;
            Admin admin = _adminRepository.GetAdmins().FirstOrDefault(x => x.AdminId == adminId);
            if (admin != null)
            {
                var image = Path.Combine(coverImagesPath, admin.AdminId) + ".png";
                File.Delete(image);
                _adminRepository.DeleteAdmin(admin);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Inserts the token.
        /// </summary>
        /// <param name="adminViewModel">The admin view model.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public bool InsertToken(AdminViewModel adminViewModel, string host)
        {
            Admin admin = _adminRepository.GetAdmins().Where(x => x.AdminEmail ==  adminViewModel.AdminEmail).FirstOrDefault();
            if (admin != null)
            {
                admin.Token = Guid.NewGuid().ToString();
                _adminRepository.UpdateAdmin(admin);
                _emailSender.PasswordReset(admin.AdminEmail, host, admin.AdminName, admin.AdminId, admin.Token);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Checks the query paramater.
        /// </summary>
        /// <param name="AdminId">The admin identifier.</param>
        /// <param name="Token">The token.</param>
        /// <returns></returns>
        public bool CheckQueryParamater(string AdminId, string Token)
        {
            if (_adminRepository.AdminExists_v2(AdminId, Token))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="adminViewModel">The admin view model.</param>
        /// <returns></returns>
        public bool ChangePassword(AdminViewModel adminViewModel)
        {
            Admin admin = _adminRepository.GetAdmins().Where(x => x.AdminId == adminViewModel.AdminId && x.Token == adminViewModel.Token).FirstOrDefault();
            if (admin != null)
            {
                admin.Password = PasswordManager.EncryptPassword(adminViewModel.Password);
                admin.Token = null;
                _adminRepository.UpdateAdmin(admin);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Checks the email exist.
        /// </summary>
        /// <param name="adminViewModel">The admin view model.</param>
        /// <returns></returns>
        public bool CheckEmailExist(AdminViewModel adminViewModel)
        {
            Admin admin = _adminRepository.GetAdmins().FirstOrDefault(x => x.AdminEmail == adminViewModel.AdminEmail);

            if (admin != null && admin.AdminId != adminViewModel.AdminId)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Edits the admin.
        /// </summary>
        /// <param name="adminViewModel">The admin view model.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public bool EditAdmin(AdminViewModel adminViewModel, string user)
        {
			var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;
			Admin admin = _adminRepository.GetAdmins().Where(x => x.AdminId == adminViewModel.AdminId).FirstOrDefault();
            if (admin != null)
            {
                admin.AdminName = adminViewModel.AdminName;
                admin.ContactNo = adminViewModel.ContactNo;
                admin.Dob = adminViewModel.Dob;
                admin.AdminEmail = adminViewModel.AdminEmail;
                admin.Role = adminViewModel.Role;
                admin.UpdatedBy = user;
                admin.UpdatedTime = DateTime.Now;

                if (adminViewModel.AdminProfile != null)
                {
					var coverImageFileName = Path.Combine(coverImagesPath, admin.AdminId) + ".png";
					using (var fileStream = new FileStream(coverImageFileName, FileMode.Create))
					{
						adminViewModel.AdminProfile.CopyTo(fileStream);
					}
					_adminRepository.UpdateAdmin(admin);
					return true;
				}
                _adminRepository.UpdateAdmin(admin);
                return true;
            }
            return false;
        }
        #region private methods

        /// <summary>
        /// Checks the admin profile.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns></returns>
        private static bool CheckAdminProfile(string adminId)
        {
            var coverImagesPath = PathManager.DirectoryPath.CoverImagesDirectory;
            var coverImageFileName = Path.Combine(coverImagesPath, adminId) + ".png";
            if (File.Exists(coverImageFileName))
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
