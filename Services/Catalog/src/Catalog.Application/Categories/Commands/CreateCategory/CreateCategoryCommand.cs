using Catalog.Application.Common.Authorization;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Categories.Commands.CreateCategory
{
    [Authorize(Roles = "Admin")]
    public record CreateCategoryCommand(string Name ,string Description) : IRequest<ErrorOr<Category>>;
    
}
