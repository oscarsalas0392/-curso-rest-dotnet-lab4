using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private ServicesConfiguration Services { get; }
        private ILogger<OrdersController> _logger;

        public OrdersController(IConfiguration configuration, ILogger<OrdersController> logger)
        {
            _logger = logger;
            Services = new ServicesConfiguration();
            configuration.GetSection("Services").Bind(Services);
        }

        // GET: api/<Controller>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var httpClient = new HttpClient();


            var json = await httpClient.GetStringAsync(Services.ProductsSvc + "/products");
            JArray products = JArray.Parse(json);

            _logger.LogInformation($"JSON obtenido: {json}");
            var random = new Random((int)DateTime.Now.Ticks);
            
            var lines = 
                products
                .Select(line => new {
                    Quantity = random.Next(1, 9),
                    Product = new { 
                        Id = (int)line["id"],
                        Name = (string)line["name"],
                        ListPrice = (decimal)line["listPrice"],
                    }
                }
                )
                .Take(3) 
                .ToArray();

          
            object order1 = new { 
                Lines = lines, 
                Customer = new object(), 
                CreatedDate = DateTime.Now,
                // Calculamos el total con base en las lineas
                Total = lines.Sum(entry => entry.Quantity * entry.Product.ListPrice),
            };

            return Ok(new
                object[] { order1 }
            );
        }

        private class ServicesConfiguration
        {

            public string ProductsSvc { get; set; }
        }
    }
}
