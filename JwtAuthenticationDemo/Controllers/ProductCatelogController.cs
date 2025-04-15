using System.Net;
using JwtAuthenticationDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationDemo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCatelogController : ControllerBase
    {
        private readonly EShopConfigDbContext _context;

        public ProductCatelogController(EShopConfigDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductCatalog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCatalog>>> GetProductCatalogs()
        {
            return await _context.ProductCatalogs.ToListAsync();
        }

        // GET: api/ProductCatalog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCatalog>> GetProductCatalog(int id)
        {
            var productCatalog = await _context.ProductCatalogs.FindAsync(id);

            if (productCatalog == null)
            {
                return NotFound();
            }

            return productCatalog;
        }

        // PUT: api/ProductCatalog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCatalog(int id, ProductCatalog productCatalog)
        {
            if (id != productCatalog.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(productCatalog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCatalogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductCatalog
        [HttpPost]
        public async Task<ActionResult<ProductCatalog>> PostProductCatalog(ProductCatalog productCatalog)
        {
            _context.ProductCatalogs.Add(productCatalog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCatalog", new { id = productCatalog.ProductId }, productCatalog);
        }

        // POST: api/ProductCatalog
        [HttpPost("All")]        
        public async Task<ActionResult<HttpStatusCode>> PostAllProductCatalog(List<ProductCatalog> productCatalog)
        {
            _context.ProductCatalogs.AddRange(productCatalog);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/ProductCatalog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCatalog(int id)
        {
            var productCatalog = await _context.ProductCatalogs.FindAsync(id);
            if (productCatalog == null)
            {
                return NotFound();
            }

            _context.ProductCatalogs.Remove(productCatalog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductCatalogExists(int id)
        {
            return _context.ProductCatalogs.Any(e => e.ProductId == id);
        }
    }
}
