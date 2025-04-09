using System.Diagnostics;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
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
        var spec = new ProdcutSpecification(brand, type, sort);
        var products = await repo.ListAsync(spec);
        return Ok(products);
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
        var spec = new BrandListSpecification();
        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes() {
        var spec = new TypeListSpecification();
        return Ok(await repo.ListAsync(spec));
    }

    private bool ProductExists(int id) {
        return repo.Exists(id);
    }
}
