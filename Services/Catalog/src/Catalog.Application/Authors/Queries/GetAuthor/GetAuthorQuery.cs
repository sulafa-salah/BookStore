using Catalog.Domain.AuthorAggregate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Authors.Queries.GetAuthor;
    public record GetAuthorQuery(Guid AuthorId) : IRequest<ErrorOr<Author>>;