using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Authors.Commands.CreateAuthor
{
    public record CreateAuthorCommand(string Name, string Bio) : IRequest<ErrorOr<Author>>;

}
