using Catalog.Application.Categories.Commands.CreateCategory;
using Catalog.Application.Categories.Queries.GetCategory;
using Catalog.Application.Categories.Queries.ListCategories;
using Catalog.Application.Common.Mappings;
using Catalog.Contracts.Categories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Catalog.Api.Controllers;
[Route("api/[controller]")]
public class CategoriesController : ApiController
{
    private readonly ISender _mediator;

    public CategoriesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description);

        var createCategoryResult = await _mediator.Send(command);

        return createCategoryResult.Match(
            category => Ok(new CategoryResponse(category.Id, category.Name, category.Description)),
            Problem);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetById(Guid categoryId)
    {

        var command = new GetCategoryQuery(categoryId);

        var getCategoryResult = await _mediator.Send(command);
        return getCategoryResult.Match(
            c => Ok(new CategoryResponse(c.Id, c.Name, c.Description)),
            errors => Problem(errors) // your ApiController maps NotFound → 404, etc.
        );
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] GetCategoriesRequest req, CancellationToken ct)
    {
        var listCategoriesResult = await _mediator.Send(new ListCategoriesQuery(
            req.PageNumber, req.PageSize, req.Search, req.SortBy, req.SortDir), ct);

        return listCategoriesResult.Match(

        paged => Ok(paged.ToResponse(c =>
      new CategoryResponse(c.Id, c.Name, c.Description))),
    errors => Problem(errors)
        );
    }
}

