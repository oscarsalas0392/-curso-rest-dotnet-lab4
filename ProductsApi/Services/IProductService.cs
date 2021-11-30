using Newtonsoft.Json.Linq;

namespace ProductsApi.Services
{
    public interface IProductService
    {
        object[] Get();
        object Get(int id);

        (int id, object resource) Insert(string productName);
    }   
}