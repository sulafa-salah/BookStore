using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name ,string Description) : IRequest<ErrorOr<Category>>;
    
}
