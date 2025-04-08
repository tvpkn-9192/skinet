using System.Diagnostics;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IGenericRepository<Product> repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, string? sort) {
        return Ok(await repo.ListAllAsync());
    }    

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id) {
        var product = await repo.GetByIdAsync(id);

        if (product == null) {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product) {
        repo.Add(product);
        if(await repo.SaveAllAsync()) {
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

        repo.Update(product);

        if(await repo.SaveAllAsync()) {
            return NoContent();
        }

        return BadRequest("Problem While updating product ");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id) {
        var product = await repo.GetByIdAsync(id);
        if (product == null) {
            return NotFound();
        }

        repo.Remove(product);

        if(await repo.SaveAllAsync()) {
            return NoContent();
        }

        return BadRequest("Problem While deleting product ");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands() {
        //TODO: Implement this method in the repository
        return Ok();
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes() {
        //TODO: Implement this method in the repository
        return Ok();
    }

    private bool ProductExists(int id) {
        return repo.Exists(id);
    }
}
