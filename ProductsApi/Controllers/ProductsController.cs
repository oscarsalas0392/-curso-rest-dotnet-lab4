using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductsApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductsApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductService _service;
        private ILogger<ProductsController> _logger;

        // Create->Read->Update->Delete
        // Post  ->Get ->Put   ->Delete
        public ProductsController(
            ILogger<ProductsController> logger,
            IProductService service
        )
        {
            _service = service;
            _logger = logger;

            _logger.LogInformation("DI correcto");
        }

        // GET: api/<ProductsController>
        [HttpGet]
        public ActionResult Get()
        {
            _logger.LogInformation("Llamado del GET ejecutandose");
            return Ok(_service.Get());
        }

        // GET api/products/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            // 404->NotFound
            var found = _service.Get(id);

            if (found == null)
            {
                return NotFound("No se encontro el producto");
            }

            return Ok(found);
        }


        public class ProductRequest 
        {
            int Id { get; set; }

            string Name { get; set; }

            decimal ListPrice { get; set; }
        }

        // POST api/<ProductsController>
        [HttpPost]
        public ActionResult Post()
        {
            string body = new System.IO.StreamReader(Request.Body).ReadToEndAsync().GetAwaiter().GetResult();

            var json = JsonConvert.DeserializeObject<JObject>(body);
            string productName = json.GetValue("name")?.Value<string>() ?? string.Empty;

            if (string.IsNullOrEmpty(productName))
            {
                // TODO: Retornar como JSON
                return BadRequest("Atributo 'name' es requerido");
            }

            // Deconstruct
            (int id, object resource) = _service.Insert(productName);

            return CreatedAtAction(nameof(Get), new { id = id }, resource); //Ok();
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] ProductRequest request)
        {
            // Checkear que exista el id
            // Si no, return 404
            // En caso que si, llamar al Update del servicio

            return Ok();
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            // Checkear que exista el id
            // Si no, return 404
            // En caso que si, llamar al Delete del servicio

            return Ok();
        }

        [HttpPatch]
        public ActionResult Patch(int id) => throw new NotImplementedException();
    }
}
