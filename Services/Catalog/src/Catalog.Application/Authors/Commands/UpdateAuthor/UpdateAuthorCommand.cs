using Catalog.Domain.AuthorAggregate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Authors.Commands.UpdateAuthor;

    public record UpdateAuthorCommand(Guid AuthorId, string Name, string Bio, bool IsActive = true) : IRequest<ErrorOr<Author>>;