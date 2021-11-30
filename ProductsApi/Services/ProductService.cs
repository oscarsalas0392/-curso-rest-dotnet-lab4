using DataAccessLayer.Entities;
using System;
using System.Linq;

namespace ProductsApi.Services
{
    public class ProductService : IProductService
    {
        private AdventureworksContext _dbContext;

        public ProductService(AdventureworksContext dbContext)
        {
            _dbContext = dbContext;
        }

        public object[] Get()
        {
            var query = _dbContext.Products.Select(product => new {
                Id = product.ProductId,
                Name = product.Name,
                ListPrice = product.ListPrice,
            }).ToArray();

            // Extender el query
            return query;
        }

        public object Get(int id)
        {
            var dbRecord = _dbContext.Products.Find(id);

            return new
            {
                Id = dbRecord.ProductId,
                Name = dbRecord.Name,
                ListPrice = dbRecord.ListPrice
            };
        }

        public (int, object) Insert(string productName)
        {
            var productNumber = Guid.NewGuid().ToString("D").Replace("-", "").Substring(0, 25);

            Product dbRecord = new Product { 
                Name = productName,
                ModifiedDate = DateTime.UtcNow,
                SellStartDate = DateTime.UtcNow,
                ProductNumber = productNumber,
            };
            _dbContext.Products.Add(dbRecord);

            _dbContext.SaveChanges(); // async

            int id = dbRecord.ProductId;

            return (id, new {
                Id = dbRecord.ProductId,
                Name = dbRecord.Name,
                ListPrice = dbRecord.ListPrice
            });
        }
    }
}
