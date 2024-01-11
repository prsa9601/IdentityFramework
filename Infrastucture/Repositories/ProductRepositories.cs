using Domain.ProductAgg.Repository;
using Infrastructure.Persistent.Ef;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.ProductAgg;
using Infrastucture.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Domain.UserAppAgg;
using Domain.UserAgg;
using Microsoft.AspNetCore.Server.IIS.Core;
using Domain.Permission;

namespace Infrastructure.Repositories
{
    public class ProductRepositories : IProductRepository
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly MyDBContext _context;
        public ProductRepositories( MyDBContext contex, UserManager<UserApp> userManager, SignInManager<UserApp> signInManager)
        {
            _context = contex;
            _userManager = userManager;
            _signInManager = signInManager;
        }




        public List<Product> ReadProductByUser(string username)
        {
            try
            {
                var Product = _context.product.Where(i => i.UserName == username && i.DeleteStatus == false).Select(i => i);
                return Product.ToList();
            }
            catch 
            {
                return null;
            }
        }

     

        public List<Product> ReadProduct()
        {
            try
            {
                var Product = _context.product.Where(i => i.DeleteStatus == false).ToList();
                if (Product != null)
                {
                    return Product;
                }
                return null;
            }
            catch 
            {
                return null;
            }
        }

        public string GetById(int id)
        {
            throw new NotImplementedException();
        }

     

        public async Task<string> CreateProduct(Product product)
        {
            try
            {
                product.ProduceDate = DateTime.Now;
                product.Is_A_Valiable = true;
                _context.product.Add(product);
                await _context.SaveChangesAsync();
                var Product = _context.product.Where(i => i.Id == product.Id).FirstOrDefault();
                var Roles = _context.Roles.Where(i => i.Name == "Admin").FirstOrDefault();

                if (Roles != null)
                {
                    var permission = new Permission
                    {
                        RoleName = Roles.Name,
                        ProductId = Product.Id,
                        ProductName = Product.Name,
                        Products = Product,
                        PermissionDelete = true,
                        PermissionUpdate = true,
                        DeleteStatus = false,
                        RoleId = Roles.roleId,
                    };
                    _context.permissions.Add(permission);
                    await _context.SaveChangesAsync();
                }
                  return "عملیات با موفقیت انجام شد.";
            
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public List<Product> FilterProductByUserName(string username)
        {
            try
            {
                var Product = _context.product.Where(i => i.UserName == username && i.DeleteStatus == false).Select(i => i);
                return Product.ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<Product> ReadProductByUserNow(string UserId)
        {
            try
            {
                var Product = _context.product.Where(i => i.userId == UserId && i.DeleteStatus == false).Select(i => i);
                return Product.ToList();
            }
            catch
            {
                return null;
            }
        }
       
        public async Task<string> DeleteProduct(int ProductId, string Username)
        {
            try
            {
                var GetProductByUserName = _context.product.Where(i => i.UserName == Username).FirstOrDefault();
                var Product = _context.product.Where(i => i.Id == ProductId).FirstOrDefault();
                var Users = _context.Users.Where(p => p.UserName == Username).FirstOrDefault();
                if(GetProductByUserName != null)
                {
                    Product.DeleteStatus = true;
                    await _context.SaveChangesAsync();
                    return "عملیات با موفقیت انجام شد.";
                }
                else if (Product == null)
                {
                    return "همچین محصولی یافت نشد!";
                }
                var Roles = _context.Roles.Where(p => p.Name == Users.Role).FirstOrDefault();
                if (Roles == null) return "تمامی فیلد ها را پر کنید";
                var Permission = _context.permissions.Where(p => p.ProductId == ProductId && p.RoleId == Roles.roleId).FirstOrDefault();
                if (Permission == null) return "تمامی فیلد ها را پر کنید";
                else if (Permission.PermissionDelete || Permission.RoleName == "Admin")
                {
                        Permission.DeleteStatus = true;
                        Product.DeleteStatus = true;
                        await _context.SaveChangesAsync();
                        return "عملیات با موفقیت انجام شد.";
                }
              
                return "حین عملیات با مشکلی مواجه شدیم!!!";
            }
            catch (Exception ex)
            { 
                return ex.Message;
            }
        }
        public async Task<string> UpdateProduct(int ProductId, string Username, string productname, string price)
        {
            try
            {
                var Product = _context.product.Where(i => i.UserName == Username).FirstOrDefault();
                var GetProductById = _context.product.Where(i => i.Id == ProductId).FirstOrDefault();
                if (Product != null)
                {
                    GetProductById.Name = productname;
                    GetProductById.Price = price;
                    await _context.SaveChangesAsync();
                    return "عملیات با موفقیت انجام شد.";
                }
                var User = _context.Users.Where(p => p.UserName == Username).FirstOrDefault();
                if (User == null) return "تمامی فیلد ها را پر کنید";
                var Roles = _context.Roles.Where(p => p.Name == User.Role).FirstOrDefault();
                if (Roles == null) return "تمامی فیلد ها را پر کنید";
                var Permission = _context.permissions.Where(p => p.ProductId == ProductId && p.RoleId == Roles.roleId).FirstOrDefault();
                if (Permission == null) return "تمامی فیلد ها را پر کنید";
                else if (Product != null || Permission.PermissionUpdate || Permission.RoleName == "Admin")
                {
                    Permission.ProductName = productname;
                    GetProductById.Name = productname;
                    GetProductById.Price = price;
                    await _context.SaveChangesAsync();
                    return "عملیات با موفقیت انجام شد.";
                }
                return "حین عملیات با مشکلی مواجه شدیم!!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public bool UserOwnsProduct(int Productid, string userId)
        {
            try
            {
                var Product = _context.product.Where(i => i.Id == Productid).FirstOrDefault();
                if (Product == null) return false;
                if (Product.userId == userId)
                {
                    return true;
                }
                else if (Product == null)
                {
                    return false;
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
    }
}
