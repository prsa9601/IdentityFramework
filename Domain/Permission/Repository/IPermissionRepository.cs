using Domain.ProductAgg;
using Domain.RoleAgg;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Permission.Repository
{
    public interface IPermissionRepository
    {
      
        Task<string> CreatePermission(int ProductId, string Role, bool Update, bool Delete);
        string DeletePermission(int ProductId, string Role);
        Task<string> UpdatePermission(int ProductId, string Role, bool Update, bool Delete);
        List<Permission> GetAllPermission();
        Permission GetByIdPermission(int id);
        List<Permission> GetPermission(string Role);
        Permission FilterPermission(string RoleName, int ProductId);
    }
}
