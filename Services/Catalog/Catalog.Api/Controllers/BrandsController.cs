using Catalog.Api.Dtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/brands")]
public sealed class BrandsController : ControllerBase
{
    private readonly IBrandRepository _brandRepository;

    public BrandsController(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBrandDto dto)
    {
        var brand = new Brand(dto.Name);

        await _brandRepository.AddAsync(brand);

        return Created(
            $"/api/brands/{brand.Id}",
            new { brand.Id, brand.Name });
    }



    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
    {
        var brands = await _brandRepository.GetAllAsync();

        var result = brands.Select(brand =>
            new BrandDto(
                brand.Id,
                brand.Name
            ));

        return Ok(result);
    }
}