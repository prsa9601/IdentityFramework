using Domain.Permission;
using Domain.Permission.Repository;
using Domain.ProductAgg;
using Domain.RoleAgg;
using Domain.RoleAgg.Repository;
using Infrastructure.Persistent.Ef;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PermissionRepositories : IPermissionRepository
    {
        private readonly MyDBContext _context;
        public PermissionRepositories(MyDBContext context)
        {
            _context = context;
        }
  
        public async Task<string> CreatePermission(int ProductId, string role, bool update, bool delete)
        {
            try
            {
                var Permission = _context.permissions.Where(i => i.ProductId == ProductId && i.RoleName == role).FirstOrDefault();
                if (Permission == null)
                {
                    Permission permission = new Permission();
                    var Product = _context.product.Where(i => i.Id == ProductId).FirstOrDefault();
                    if (Product == null) return "تمامی فیلد ها را پر کنید";
                    var Role = _context.Roles.Where(p => p.Name == role).FirstOrDefault();
                    if (Role == null) return "تمامی فیلد ها را پر کنید";
                    permission.Roles = Role;
                    permission.Products = Product;
                    permission.RoleName = role;
                    permission.ProductId = Product.Id;
                    permission.ProductName = Product.Name;
                    permission.RoleId = Role.roleId;
                    permission.PermissionDelete = delete;
                    permission.PermissionUpdate = update;
                    _context.permissions.Add(permission);
                    await _context.SaveChangesAsync();
                    return "عملیات با موفقیت انجام شد.";
                }
                return "حین عملیات حذف با مشکلی مواجه شدیم!!!";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeletePermission(int productId, string role)
        {
            try
            {
                var permission = _context.permissions.Where(i => i.ProductId == productId && i.RoleName == role).FirstOrDefault();
                if (permission != null)
                {
                    permission.DeleteStatus = true;
                    _context.SaveChanges();
                    return "عملیات با موفقیت انجام شد.";
                }
                return "حین عملیات حذف با مشکلی مواجه شدیم!!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> UpdatePermission(int productId, string role, bool update, bool delete)
        {
            try
            {
                var Product = _context.product.Where(i => i.Id == productId).FirstOrDefault();
                if (Product == null) return "تمامی فیلد ها را پر کنید";
                var Role = _context.Roles.Where(i => i.Name == role).FirstOrDefault();
                if (Role == null) return "تمامی فیلد ها را پر کنید";
                var permission = _context.permissions.Where(i => i.ProductId == productId && i.RoleId == Role.roleId).FirstOrDefault();
                if (permission != null)
                {
                    permission.Roles = Role;
                    permission.RoleId = Role.roleId;
                    permission.PermissionDelete = delete;
                    permission.PermissionUpdate = update;
                    permission.Products = Product;
                    permission.ProductName = Product.Name;
                    permission.ProductId = Product.Id;
                    await _context.SaveChangesAsync();
                    return "عملیات با موفقیت انجام شد."; 
                }
                return "حین ویرایش عملیات با مشکلی مواجه شدیم!!!";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public List<Permission> GetAllPermission()
        {
            try
            {
                var Permission = _context.permissions.Select(i => i).Where(p => p.DeleteStatus == false).ToList();
                return Permission;
            }
            catch
            {
                return null;
            }
        }
        public Permission GetByIdPermission(int id)
        {
            try
            {
                var Permission = _context.permissions.Select(i => i).Where(p => p.DeleteStatus == false && p.id == id).FirstOrDefault();
                if (Permission != null) return Permission;
                else
                    return null;
            }
            catch 
            {
                return null;
            }
        }
        public List<Permission> GetPermission(string roleName)
        {
            try
            {
                var Permission = _context.permissions.Where(i => i.RoleName == roleName).ToList();
                return Permission;
            }
            catch
            {
                return null;
            }
        }
        public Permission FilterPermission(string roleName,int productId)
        {
            try
            {
                var Permission = _context.permissions.Where(i => i.RoleName == roleName && i.ProductId == productId).FirstOrDefault();
                return Permission;
            }
            catch
            {
                return null;
            }
        }
    }
}
