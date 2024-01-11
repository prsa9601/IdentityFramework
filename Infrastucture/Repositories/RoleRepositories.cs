using Azure.Security.KeyVault.Certificates;
using Domain.Permission;
using Domain.ProductAgg;
using Domain.RoleAgg;
using Domain.RoleAgg.Repository;
using Domain.UserAgg;
using Domain.UserAppAgg;
using Infrastructure.Persistent.Ef;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RoleRepositories : IRoleRepository
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<Role> _RoleManager;
        private readonly MyDBContext _context;
        public RoleRepositories(MyDBContext context, UserManager<UserApp> userManager, RoleManager<Role> RoleManager)
        {
            _context = context;
            _userManager = userManager;
            _RoleManager = RoleManager;
        }
        public bool Exist()
        {
            try
            {
                if ( _context.Roles.Count() == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch 
            {
                return false;
            }
        }
        public string CreateRole(Role Role)
        {
            try
            {
                    var role = _context.Roles.Where(i => i.Name == Role.Name);
                    if (role == null)
                    {
                        _context.Roles.Add(Role);
                        _context.SaveChanges();
                        return "عملیات با موفقیت انجام شد.";
                    }
                    return "حین عملیات با مشکلی مواجه شدیم!!!";
            }
            catch (Exception ex)
            {
               return ex.Message;
            }
        }
        public string DeleteAccessRole(string RoleName)
        {
            try
            {
                var role = _context.Roles.Where(i => i.Name == RoleName).FirstOrDefault();
                if (role != null)
                {
                    role.DeleteStatus = true;
                    _context.SaveChanges(); 
                    return "عملیات با موفقیت انجام شد.";
                }
                return "حین عملیات حذف مشکلی پیش اومد!!!";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public Role GetRoleByUserId(string RoleName)
        {
            try
            {
                var role = _context.Roles.Where(i => i.Name == RoleName).FirstOrDefault();
                if (role != null)
                    return role;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public List<Role> ReadRole()
        {
            try
            {
                var role = _context.Roles.Where(i => i.DeleteStatus == false).ToList();
                return role;
            }
            catch 
            {
                return null;
            }
        }
        public string UpdateRole(string RoleName, Role RoleModel)
        {
            try
            {
                var role = _context.Roles.Where(i => i.Name == RoleName).FirstOrDefault();
                if (role != null)
                {
                    role.Name = RoleModel.Name;
                    _context.SaveChanges();
                    return "عملیات با موفقیت انجام شد.";
                }
                return "حین عملیات ویرایش با مشکلی مواجه شدیم!!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteRole(string RoleName)
        {
            try
            {
                var role = _context.Roles.Where(i => i.Name == RoleName).FirstOrDefault();
                if (role != null)
                {
                    role.DeleteStatus = true;
                    _context.SaveChanges();
                    return "عملیات با موفقیت انجام شد.";
                }
                return "حین حذف با مشکلی مواجه شدیم!!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
