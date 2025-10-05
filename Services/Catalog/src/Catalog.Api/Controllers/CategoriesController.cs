using Catalog.Contracts.Categories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Categories.Commands.CreateCategory;

namespace Catalog.Api.Controllers
{
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

            return createCategoryResult.MatchFirst(
                category => Ok(new CategoryResponse(category.Id, category.Name, category.Description)),
                error => Problem());
        }
    }
}
