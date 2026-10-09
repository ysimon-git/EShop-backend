using Catalog.Api.Dtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var category = new Category(dto.Name);

        await _categoryRepository.AddAsync(category);

        return Created(
            $"/api/categories/{category.Id}",
            new { category.Id, category.Name });
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryRepository.GetAllAsync();

        var result = categories.Select(category =>
            new CategoryDto(
                category.Id,
                category.Name
            ));

        return Ok(result);
    }
}