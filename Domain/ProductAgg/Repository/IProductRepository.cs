using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProductAgg.Repository
{
    public interface IProductRepository
    {
        Task<string> UpdateProduct(int ProductId, string UserName, string ProductName, string Price);
        bool UserOwnsProduct(int Productid,string UserId);
        Task<string> DeleteProduct(int ProductId, string Username);
        Task<string> CreateProduct(ProductAgg.Product Product);
        string GetById(int id);
        List<Product> FilterProductByUserName(string UserName);
        List<Product> ReadProductByUserNow(string UserId);
        List<Product> ReadProduct();
        List<Product> ReadProductByUser(string UserName);
    }
}
