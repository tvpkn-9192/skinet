using System.Diagnostics;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
        return Ok(await repo.GetProductsAsync());
    }    

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id) {
        var product = await repo.GetProductByIdAsync(id);

        if (product == null) {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product) {
        repo.AddProduct(product);
        if(await repo.SaveChangesAsync()) {
            return CreatedAtAction("GetProduct", new { id = product.Id}, product);
        }
        return BadRequest("Problem While creating product ");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product) {
        Console.WriteLine($"product: {product}");
        if (id != product.Id || !ProductExists(id)) {
            Console.WriteLine($"product: {product}");
            return BadRequest("Cannot update this product");
        }

        repo.UpdateProduct(product);

        if(await repo.SaveChangesAsync()) {
            return NoContent();
        }

        return BadRequest("Problem While updating product ");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id) {
        var product = await repo.GetProductByIdAsync(id);
        if (product == null) {
            return NotFound();
        }

        repo.DeleteProduct(product);

        if(await repo.SaveChangesAsync()) {
            return NoContent();
        }

        return BadRequest("Problem While deleting product ");
    }

    private bool ProductExists(int id) {
        return repo.ProductExists(id);
    }
}
