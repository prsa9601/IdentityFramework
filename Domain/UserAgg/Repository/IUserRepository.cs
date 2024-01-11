using Domain.UserAppAgg;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAgg.Repository
{
    public interface IUserRepository
    {
        Task<string> Create(User UserModel);
        Task<string> Login(string UserName, string Password);
        string Delete(int id);
        bool Exist();
        string Update(int id, User User);
        List<User> Read();
        User GetByUserName(string UserName);
        bool PasswordIsCorrect(string Mobile, string Password);
        Task<string> UseRole(string UserName, string Role);
    }
}
