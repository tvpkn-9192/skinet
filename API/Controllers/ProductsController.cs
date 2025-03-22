using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController :  ControllerBase
{
    private readonly StoreContext context;

    public ProductsController(StoreContext context) {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
        return await context.Products.ToListAsync();
    }    

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id) {
        var product = await context.Products.FindAsync(id);

        if (product == null) {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product) {
        context.Products.Add(product);
        await context.SaveChangesAsync();

        return product;
        //return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product) {
        if (id != product.Id) {
            return BadRequest();
        }

        context.Entry(product).State = EntityState.Modified;

        try {
            await context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            if (!ProductExists(id)) {
                return NotFound();
            } else {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id) {
        var product = await context.Products.FindAsync(id);
        if (product == null) {
            return NotFound();
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id) {
        return context.Products.Any(e => e.Id == id);
    }
}
