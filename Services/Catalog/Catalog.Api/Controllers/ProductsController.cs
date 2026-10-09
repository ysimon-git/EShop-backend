using Catalog.Api.Dtos;
using Catalog.Api.Mappers;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(IProductRepository productRepository, IBrandRepository brandRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
        }




        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {

            if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
            {
                return BadRequest(new
                {
                    message = "La catégorie spécifiée n'existe pas."
                });
            }

            if (!await _brandRepository.ExistsAsync(dto.BrandId))
            {
                return BadRequest(new
                {
                    message = "La marque spécifiée n'existe pas."
                });
            }

            var product = new Product(
                            dto.Name,
                            dto.Description,
                            dto.Price,
                            dto.StockQuantity,
                            dto.CategoryId,
                            dto.BrandId,
                            dto.ImageUrl);

            await _productRepository.AddAsync(product);

            return Created(
                $"/api/products/{product.Id}",
                new { product.Id });
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productRepository.GetAllAsync();

            return Ok(products.Select(ProductMapper.ToDto));
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
                return NotFound();

            return Ok(ProductMapper.ToDto(product));
        }



        [HttpPost("by-ids")]
        public async Task<IActionResult> GetByIds(
          ProductIdsDto dto,
          CancellationToken cancellationToken)
        {
            // 1. Retrieve all requested products
            var products = await _productRepository.GetByIdsAsync(
                dto.ProductIds,
                cancellationToken);

            // 2. Extract the distinct CategoryIds
            var categoryIds = products
                .Select(p => p.CategoryId)
                .Distinct()
                .ToList();

            // 3. Extract the distinct BrandIds
            var brandIds = products
                .Select(p => p.BrandId)
                .Distinct()
                .ToList();

            // 4. Retrieve all required categories
            var categories = await _categoryRepository.GetByIdsAsync(
                categoryIds,
                cancellationToken);

            // 5. Retrieve all required brands
            var brands = await _brandRepository.GetByIdsAsync(
                brandIds,
                cancellationToken);

            // 6. Build the API response
            var result = products
                .Select(product =>
                {
                    var category = categories.First(c =>
                        c.Id == product.CategoryId);

                    var brand = brands.First(b =>
                        b.Id == product.BrandId);

                    return new ProductOrderDetailsDto(
                        product.Id,
                        product.Name,
                        product.ImageUrl,
                        category.Name,
                        brand.Name);
                })
                .ToList();

            return Ok(result);
        }



        [HttpPatch("{id:guid}/price")]
        public async Task<IActionResult> ChangePrice(Guid id, ChangeProductPriceDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
                return NotFound();

            product.ChangePrice(dto.Price);

            await _productRepository.UpdateAsync(product);

            return NoContent();
        }



        [HttpPost("{id:guid}/stock/add")]
        public async Task<IActionResult> AddStock(Guid id, ChangeStockDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
                return NotFound();

            product.AddStock(dto.Quantity);

            await _productRepository.UpdateAsync(product);

            return NoContent();
        }


        [HttpPost("{id:guid}/stock/remove")]
        public async Task<IActionResult> RemoveStock(Guid id, ChangeStockDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
                return NotFound();

            product.RemoveStock(dto.Quantity);

            await _productRepository.UpdateAsync(product);

            return NoContent();
        }

    }



}
