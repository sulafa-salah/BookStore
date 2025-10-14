using Catalog.Domain.BookAggregate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Queries.GetBook;
    public  record GetBookQuery(Guid BookId) : IRequest<ErrorOr<Book>>;
