using Domain.Permission;
using Domain.ProductAgg;
using Domain.RoleAgg;
using Domain.UserAgg;
using Domain.UserAppAgg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RoleAgg.Repository
{
    public interface IRoleRepository
    {
        string CreateRole(Role Role);
        List<Role> ReadRole();
        Role GetRoleByUserId(string RoleName);
        string UpdateRole(string RoleName, Role RoleModel);
        string DeleteRole(string RoleName);
        bool Exist();
        string DeleteAccessRole(string RoleName);

    }
}
