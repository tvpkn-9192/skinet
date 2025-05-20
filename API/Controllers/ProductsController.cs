using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IUnitOfWork unit) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductSpecParams specParams) {
        var spec = new ProdcutSpecification(specParams);
        return await CreatePagedResult(unit.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);
    }    

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id) {
        var product = await unit.Repository<Product>().GetByIdAsync(id);

        if (product == null) {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product) {
        unit.Repository<Product>().Add(product);
        if(await unit.Complete()) {
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

        unit.Repository<Product>().Update(product);

        if(await unit.Complete()) {
            return NoContent();
        }

        return BadRequest("Problem While updating product ");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id) {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product == null) {
            return NotFound();
        }

        unit.Repository<Product>().Remove(product);

        if(await unit.Complete()) {
            return NoContent();
        }

        return BadRequest("Problem While deleting product ");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands() {
        var spec = new BrandListSpecification();
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes() {
        var spec = new TypeListSpecification();
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }

    private bool ProductExists(int id) {
        return unit.Repository<Product>().Exists(id);
    }
}
