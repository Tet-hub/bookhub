﻿using NavOS.Basecode.Data.Models;
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
        public LoginResult AuthenticateAdmin(string email, string password, ref Admin admin);
        public void AddAdmin(AdminViewModel model);
        public Admin GetAdmin(string adminId);
		public List<Admin> GetAllAdmins();
        public bool DeleteAdmin(AdminViewModel adminViewModel);
        public bool EditAdmin(AdminViewModel adminViewModel);

	}
}
