using Azure;
using Domain.UserAgg;
using Domain.UserAgg.Repository;
using Domain.UserAppAgg;
using Domain.Utility;
using Infrastructure.Persistent.Ef;
using Infrastucture.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepositories : IUserRepository
    {
        private readonly MyDBContext _context;
        private readonly UserManager<UserApp> _userManager;
        private readonly JWTSettings _JwtSetting;
        public UserRepositories(MyDBContext context, IOptions<JWTSettings> options, UserManager<UserApp> userManager)
        {
            _context = context;
            _userManager = userManager;
            _JwtSetting = options.Value;
        }
        public async Task<string> Login(string username, string password)
        {
            try
            {
                if (_context.Users.Count() > 0)
                {
                    var UserApp = await _context.Users.SingleOrDefaultAsync(i => i.UserName == username.Trim() && i.Password == password.Trim());
                    if (UserApp == null) return null;
                    var key = Encoding.ASCII.GetBytes(_JwtSetting.Secret);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenDescription = new SecurityTokenDescriptor()
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    //new Claim(ClaimTypes.Name, UserApp.Id.ToString()),
                    new Claim(ClaimTypes.Name, UserApp.Id.ToString()),
                    new Claim(ClaimTypes.Role, UserApp.Role.ToString()),

                        }),
                        Expires = DateTime.Now.AddDays(7),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature
                            ),
                        Issuer = _JwtSetting.Issuer,
                        Audience = _JwtSetting.Audience
                    };
                    var token = tokenHandler.CreateToken(tokenDescription);
                    UserApp.JwtSecret = tokenHandler.WriteToken(token);
                    return UserApp.JwtSecret;
                }
                else
                    return "هیچ کاربری در دیتابیس ذخیره نشده!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public bool Exist()
        {
            try
            {
                if (_context.Users.Count() == 0)
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
        public bool PasswordIsCorrect(string mobile, string pass)
        {
            try
            {
                if (_context.Users.Count() > 0)
                {
                    return _context.Users.Any(c => c.UserName == mobile.Trim() &&
                    c.Password == pass
                    );
                }
                else
                    return false;
            }
            catch 
            {
                return false;
            }
        }
        public async Task<string> Create(User UserModel)
        {
            try
            {
                
                    var UserApp = _context.Users.FirstOrDefault(x => x.UserName == UserModel.UserName);
                    if (UserApp == null)
                    {
                        _context.user.Add(UserModel);
                        await _context.SaveChangesAsync();
                        return "عملیات با موفقیت انجام شد.";
                    }
                    return "نام کاربری تکراری است!";
            }
            catch
            {
                return "مشکلی در ذخیره اطلاعات رخ داده است!!!";
            }
        }
        public string Delete(int id)
        {
            try
            {
                var User = _context.user.Where(i => i.id == id).FirstOrDefault();
                if (User != null)
                {
                    User.DeleteStatus = true;
                    _context.SaveChanges();
                    return "عملیات با موفقیت انجام شد.";

                }
                return "حین حذف مشکلی پیش اومد!!!";
            }
            catch(Exception ex) 
            {
                return ex.Message;
            }
        }
        public string Update(int id,User userMe)
        {
            try
            {
                var User = _context.user.Where(i => i.id == id).FirstOrDefault();
                if (User != null)
                {
                    User.Name = userMe.Name;
                    User.UserName = userMe.UserName;
                    User.PhoneUser = userMe.PhoneUser;
                    User.Email = userMe.Email;
                    User.Password = userMe.Password;
                    _context.SaveChanges();
                    return "عملیات با موفقیت انجام شد.";
                }
                return "حین ویرایش با مشکلی مواجه شدیم!!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public User GetByUserName(string username)
        {
            try
            {
                if (_context.user.Count() > 0)
                {
                    return _context.user.Where(i => i.UserName == username).FirstOrDefault();
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public List<User> Read()
        {
            try
            {
                var User = _context.user.Where(i => i.DeleteStatus == false).Select(i => i).ToList();
                if (User != null)
                {
                    return User;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> UseRole(string username, string role)
        {
            try
            {
                var UserApp = _context.Users.Where(i => i.UserName == username).FirstOrDefault();
                if (UserApp != null)
                {
                    UserApp.Role = role;
                    await _context.SaveChangesAsync();
                    //return "";
                    return "عملیات با موفقیت انجام شد.";
                }
                //return "";
                return "در حین عملیات با مشکلی مواجه شدیم!!!";
            }
            catch(Exception ex) 
            {
                return ex.Message;
            }
        }
    }
}
